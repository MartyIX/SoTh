using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sokoban.View;
using AvalonDock;
using System.Collections.ObjectModel;
using Sokoban;
using Sokoban.Model;
using Sokoban.View.ChooseConnection;
using System.Diagnostics;
using Sokoban.Lib;
using Sokoban.Configuration;
using Sokoban.Model.Quests;
using Sokoban.Model.GameDesk;
using Sokoban.Lib.Exceptions;
using Sokoban.Interfaces;
using Sokoban.View.SetupNetwork;


namespace Sokoban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : System.Windows.Window, IQuestHandler
    {
        public ReadOnlyObservableCollection<string> MyProperty { get; set; }

        public MainWindow()
        {
            // Application initialization (processing cmd-line, splash, ...)
            ApplicationRepository.Instance.OnStartUp();

            InitializeComponent();
            this.DataContext = this;

            DebuggerIX.Start(DebuggerMode.File);
        }

        //
        // Window events
        //


        /// <summary>
        /// After the user is logged in (or he/she skips the dialog window)
        /// </summary>
        public void OnStartUp_PhaseThree()
        {
            questsPane.Initialize(this, consolePane, ProfileRepository.Instance);
            pendingGamesPane.Initialize(consolePane, ProfileRepository.Instance);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
                                 
        }        


        private void Window_Closed(object sender, EventArgs e)
        {
            DebuggerIX.Close();
        }

        private void loadQuest()
        {
            string result = Sokoban.Properties.Resources.TestQuest;
            gameManager.Add(result);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                loadQuest();
                e.Handled = true;
            }
            else if (e.Key == Key.R)
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.Reload();
                }
            }
            else if (e.Key == Key.T)
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.DisplayBothDesks = !gameManager.ActiveGameControl.DisplayBothDesks;
                }
            }
            else
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.KeyIsDown(sender, e);
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            gameManager.ActiveGameControl.KeyIsUp(sender, e);        
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ApplicationRepository.Instance.OnStartUp_PhaseTwo(); // initialize QuestsControl
            solversPane.Initialize(this.gameManager, this, consolePane );            

            // Load testing quest
            this.loadQuest();                  
        }


        /// <summary>
        /// Correctly terminates everything in the main window that needs it
        /// </summary>
        private void Terminate()
        {
            if (gameManager != null)
            {
                gameManager.Terminate();
            }

            if (solversPane != null)
            {
                solversPane.Terminate(); // unload dynamic libraries
            }
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Terminate();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Terminate();
        }

        //
        // MENU CLICKS HANDLERS
        //

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }


        private void MenuItem_Console_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(consolePane);
        }

        private void MenuItem_Solvers_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(solversPane);
        }

        private void MenuItem_Leagues_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(questsPane);
        }

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            ApplicationRepository.Instance.LoadViewSettings();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }



        private void SetVisibilityOfMenuItems(DockableContent dc)
        {            
            if (dc.Visibility == Visibility.Visible) // the value is set in ConvertBack of AvalonDockVisibilityConverter!!!
            {
                dc.Show();
            }
            else
            {
                dc.Hide();
            }
        }

        private void SetVisibilityOfDockableContents(DockableContent dc, DockableContentState state)
        {
            if (state == DockableContentState.Hidden)
            {
                dc.Visibility = Visibility.Hidden;
            }
        }

        private void solversPane_StateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisibilityOfDockableContents(solversPane, solversPane.State);
        }

        private void questsPane_StateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisibilityOfDockableContents(questsPane, questsPane.State);
        }


        private void consolePane_StateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisibilityOfDockableContents(consolePane, consolePane.State);
        }

        #region IQuestHandler Members

        public void QuestSelected(int leaguesID, int roundsID, IQuest quest, GameMode gameMode)
        {
            bool wasOpened = true;

            try
            {
                this.gameManager.QuestSelected(leaguesID, roundsID, quest, gameMode);
            }
            catch (NotValidQuestException e)
            {
                MessageBox.Show("The league you've chosen cannot be run. More information in Console.");
                consolePane.ErrorMessage(ErrorMessageSeverity.Medium,
                    "GameManager", "The league you've chosen cannot be run. Additional message: " + e.Message);

                wasOpened = false;
            }

            if (wasOpened == true && gameMode == GameMode.TwoPlayers)
            {
                InitConnection ic = new InitConnection(leaguesID, roundsID, ProfileRepository.Instance, consolePane);
                ic.Show();
            }
        }

        #endregion
    }

    [ValueConversion(/* sourceType */ typeof(System.Windows.Visibility), /* targetType */ typeof(bool))]
    public class AvalonDockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            Debug.Assert(targetType == typeof(bool));

            Visibility visibility = (Visibility)value;
            return visibility != Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;            
            return val ? Visibility.Visible : Visibility.Hidden;
        }
    }
}