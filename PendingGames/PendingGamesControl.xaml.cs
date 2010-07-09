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


        public PendingGamesControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void Initialize(IErrorMessagesPresenter errorPresenter, IProfileRepository profileRepository)
        {
            this.profile = profileRepository;
            this.errorPresenter = errorPresenter;
            this.refresh();
        }


        private void refresh()
        {
            this.Status = "Connecting to the server";

            // it correctly displays the error
            string output = null;
            try
            {
                output = this.getRequestOnServer("/remote/GetPendingGames/");
            }
            catch (UninitializedException e)
            {
                this.Status = e.Message;
                output = null;
            }


            if (output != "error" && output != null)
            {
                GetPendingGamesXmlServerResponse response = new GetPendingGamesXmlServerResponse();

                try
                {
                    response.Parse(output);
                    this.Status = "Pending games loaded.";
                }
                catch (InvalidStateException e)
                {
                    this.Status = e.Message;
                }

                if (this.dataGridItemsSource != null)
                {
                    this.dataGridItemsSource.Clear();
                }

                this.dataGridItemsSource = new ObservableCollection<OfferItemData>(response.GameList);
                Notify("DataGridItemsSource");
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyDataGrid.SelectedCells.Count > 0)
            {
                DataGridCell cell = DataGridHelper.GetCell(MyDataGrid.SelectedCells[0]);
                int rowIndex = DataGridHelper.GetRowIndex(cell);
                
                //MyDataGrid.Items[rowIndex]
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
}

