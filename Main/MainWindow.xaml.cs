using System;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using Sokoban.Model;
using Sokoban.Lib;
using Sokoban.Configuration;
using Sokoban.Interfaces;
using Sokoban.Networking;
using Sokoban.View.Settings;
using Sokoban.View.SetupNetwork;


namespace Sokoban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public ReadOnlyObservableCollection<string> MyProperty { get; set; }
        private IUserInquirer userInquirer;

        public MainWindow()
        {
            // Application initialization (processing cmd-line, splash, ...)
            ApplicationRepository.Instance.OnStartUp();

            userInquirer = new UserInquirer(this);

            InitializeComponent();
            this.DataContext = this;
        }

        //
        // Window events
        //


        /// <summary>
        /// After the user is logged in (or he/she skips the dialog window)
        /// </summary>
        public void OnStartUp_PhaseThree()
        {
            // Close application if user is not logged in
            if (ProfileRepository.Instance.Username == "Anonymous")
            {
                this.Close();
            }
            else
            {
                questsPane.Initialize(this, consolePane, ProfileRepository.Instance);
                pendingGamesPane.Initialize(consolePane, ProfileRepository.Instance, this, this);
            }
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationRepository.Instance.appParams.DebuggerPath != null)
            {
                DebuggerIX.Initialize(ApplicationRepository.Instance.appParams.DebuggerPath);
                DebuggerIX.Start(DebuggerMode.File);

                if (ApplicationRepository.Instance.appParams.DebuggerIX_Tags != null)
                {
                    DebuggerIX.Exclude(ApplicationRepository.Instance.appParams.DebuggerIX_Tags);
                }
            }               
        }        


        /// <summary>
        /// Testing method
        /// </summary>
        private void loadQuest()
        {
            string result = Sokoban.Properties.Resources.TestQuest;
            gameManager.Add(result);
        }

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSettingsManagement.IsSavingAppLayoutEnabled)
            {
                if (UserSettingsManagement.WindowLayout != "")
                {                    
                    StringReader sr = new StringReader(UserSettingsManagement.WindowLayout);
                    dockingManager.RestoreLayout(sr);

                    SetVisibilityOfMenuItems(consolePane);
                    SetVisibilityOfMenuItems(solversPane);
                    SetVisibilityOfMenuItems(questsPane);
                    SetVisibilityOfMenuItems(pendingGamesPane);
                }
            }
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ApplicationRepository.Instance.OnStartUp_PhaseTwo(); // initialize QuestsControl
            solversPane.Initialize(this.gameManager, this, consolePane, this.userInquirer);
            gameManager.Initialize(this.userInquirer); // has to be called before a GameDeskControl is opened

            // Load testing quest
            //this.loadQuest();


            // Play network game; specified in cmd-line
            if (ApplicationRepository.Instance.appParams.NetworkGame != null)
            {
                string roundID = ApplicationRepository.Instance.appParams.NetworkGame["RoundID"];
                string leagueID = ApplicationRepository.Instance.appParams.NetworkGame["LeagueID"];

                questsPane.OpenGame(int.Parse(leagueID), int.Parse(roundID), GameMode.TwoPlayers);
            }            
        }

        void u_Completed(string s)
        {
            MessageBox.Show(s);
        }



        


    }    
}