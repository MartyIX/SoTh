using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sokoban.Model;
using Sokoban.Lib.Exceptions;
using Sokoban.Interfaces;
using System.Net;
using Sokoban.Lib.Http;
using Sokoban.Model.Xml;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System.Reflection;
using Sokoban.View.SetupNetwork;
using Sokoban.Lib;
using System.Windows.Threading;
using System.Diagnostics;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for PendingGames.xaml
    /// </summary>
    public partial class PendingGamesControl : DockableContent, INotifyPropertyChanged
    {
        //
        // API
        //

        public ObservableCollection<OfferItemData> DataGridItemsSource { get { return dataGridItemsSource; } }
        public string Status
        {
            get { return status; }
            set { status = value; Notify("Status"); }
        }

        //
        // Private fields
        //
        private string status = "";
        private ObservableCollection<OfferItemData> dataGridItemsSource = null;
        private IProfileRepository profile;
        private IErrorMessagesPresenter errorPresenter = null;
        private IProfileRepository profileRepository = null;
        private IConnectionRelayer connectionRelayer = null;
        private IConnectionDialogPresenter connectionDialogPresenter = null;
        private DispatcherTimer reloadTimer;
        private BackgroundWorker dataLoader;

        public PendingGamesControl()
        {
            InitializeComponent();
            this.DataContext = this;

            dataLoader = new BackgroundWorker();
            dataLoader.DoWork += new DoWorkEventHandler(dataLoader_DoWork);
            dataLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(dataLoader_RunWorkerCompleted);

            reloadTimer = new DispatcherTimer();
            reloadTimer.Tick += new EventHandler(reloadTimer_Tick);
            reloadTimer.Interval = new TimeSpan(0, 0, 10);

            this.IsActiveContentChanged += new EventHandler(PendingGamesControl_IsActiveContentChanged);
        }

        void PendingGamesControl_IsActiveContentChanged(object sender, EventArgs e)
        {
            if (this.IsActiveContent == false)
            {
                changeRefreshing(false); // stop refreshing
            }
        }

        public void Initialize(IErrorMessagesPresenter errorPresenter,
            IProfileRepository profileRepository, IConnectionRelayer connectionRelayer,
            IConnectionDialogPresenter connectionDialogPresenter)
        {
            this.profile = profileRepository;
            this.profileRepository = profileRepository;
            this.errorPresenter = errorPresenter;
            this.connectionRelayer = connectionRelayer;
            this.connectionDialogPresenter = connectionDialogPresenter;
            this.Refresh();
        }

        private void reloadTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void dataLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // it correctly displays the error
            string output = null;
            string error = "";
            try
            {
                output = this.getRequestOnServer("/remote/GetPendingGames/");
            }
            catch (UninitializedException ex)
            {
                output = null;
                error = ex.Message;
            }

            e.Result = new DataLoaderResult(output, error);
        }

        private void dataLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataLoaderResult dlr = e.Result as DataLoaderResult;
            string output = dlr.Data;

            if (dlr.Error != "")
            {
                this.Status = dlr.Error;
            }
            else
            {
                this.Status = "Pending games reloaded.";
            }

            if (output != "error" && output != null)
            {
                GetPendingGamesXmlServerResponse response = new GetPendingGamesXmlServerResponse();

                try
                {
                    response.Parse(output);
                    this.Status = "Pending games loaded.";
                }
                catch (InvalidStateException ex)
                {
                    this.Status = ex.Message;
                }

                if (this.dataGridItemsSource != null)
                {
                    this.dataGridItemsSource.Clear();
                }

                this.dataGridItemsSource = new ObservableCollection<OfferItemData>(response.GameList);
                Notify("DataGridItemsSource");
            }
        }


        public bool Refresh()
        {
            if (dataLoader.IsBusy)
            {
                return false;
            }
            else
            {
                this.Status = "Connecting to the server";
                dataLoader.RunWorkerAsync();
                return true;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">i.e., "/remote/GetInitLeagues/"</param>
        /// <returns>Response of the server</returns>
        private string getRequestOnServer(string request)
        {
            if (profile == null) throw new UninitializedException("Server name was not initialized.");

            string output;

            if (profile.Server == String.Empty)
            {
                output = "error";
                this.Status = "User is not logged in.";
                errorMessage(ErrorMessageSeverity.Low, "User is not logged in. Please log in and click on 'Refresh'");
            }
            else
            {
                string url = profile.Server.TrimEnd(new char[] { '/' }) + request;

                try
                {
                    output = HttpReq.GetRequest(url, "");
                }
                catch (WebException e)
                {
                    output = "error";
                    this.Status = "Error in communication with the server. Please try again in a while.";
                    errorMessage(ErrorMessageSeverity.Medium, "Error in communication with the server. Additional information: " + e.Message);
                }
                catch (Exception e)
                {
                    output = "error";
                    this.Status = "Unknown error occured. Please try again in a while.";
                    errorMessage(ErrorMessageSeverity.High, "Unknown error in communication with the server. Additional information: " + e.Message);
                }

            }

            return output;
        }

        private void errorMessage(ErrorMessageSeverity ems, string message)
        {
            if (errorPresenter != null)
            {
                errorPresenter.ErrorMessage(ems, "PendingGames", message);
            }
        }

        private void btnAutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            changeRefreshing();
        }

        private void changeRefreshing()
        {
            changeRefreshing(null);
        }

        private void changeRefreshing(bool? setState)
        {
            if (!setState.HasValue)
            {
                reloadTimer.IsEnabled = !reloadTimer.IsEnabled;
            }
            else
            {
                reloadTimer.IsEnabled = setState.Value;
            }

            if (reloadTimer.IsEnabled == true)
            {
                btnAutoRefresh.Content = "Stop refreshing";
                Refresh();
            }
            else
            {
                btnAutoRefresh.Content = "Start refreshing";
            }
        }


        private void contextMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.Refresh() == false)
            {
                this.Status = "Data are being downloaded. Wait a second.";
            }
        }

        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyDataGrid.SelectedCells.Count > 0)
            {
                DataGridCell cell = DataGridHelper.GetCell(MyDataGrid.SelectedCells[0]);
                int rowIndex = DataGridHelper.GetRowIndex(cell);

                OfferItemData o = MyDataGrid.Items[rowIndex] as OfferItemData;

                if (connectionDialogPresenter != null)
                {
                    connectionDialogPresenter.Show(o.IPAddress, o.Port, o.LeaguesID, o.RoundsID,
                        this.profileRepository, this.errorPresenter, this.connectionRelayer, null);

                    changeRefreshing(false); // turn of refreshing
                }
                else
                {
                    MessageBox.Show("Error: Cannot show 'Connection dialog'.");
                    DebuggerIX.WriteLine(DebuggerTag.AppComponents, "[PendingGamesControl]",
                        "MouseDoubleClick - ERROR: connectionDialogPresenter is NULL");
                }
            }
        }
    }


    public static class DataGridHelper
    {
        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }

            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }

        public static int GetRowIndex(DataGridCell dataGridCell)
        {
            // Use reflection to get DataGridCell.RowDataItem property value.
            PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

            DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

            return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
        }

        public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
        {
            if (VisualTreeHelper.GetParent(dataGridPart) == null)
            {
                throw new NullReferenceException("Control is null.");
            }
            if (VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
            {
                return (DataGrid)VisualTreeHelper.GetParent(dataGridPart);
            }
            else
            {
                return GetDataGridFromChild(VisualTreeHelper.GetParent(dataGridPart));
            }
        }
    }

    public class DataLoaderResult
    {
        public string Data { get; set; }
        public string Error { get; set; }

        public DataLoaderResult(string data, string error)
        {
            Data = data;
            Error = error;
        }
    }
}

