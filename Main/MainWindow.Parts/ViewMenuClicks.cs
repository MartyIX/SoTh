using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AvalonDock;
using Sokoban.View.AboutDialog;
using Sokoban.Configuration;
using System.Diagnostics;
using Sokoban.Model;

namespace Sokoban
{
    public partial class MainWindow
    {

        public string ViewSoundState
        {
            get { return (UserSettingsManagement.IsSoundEnabled) ? "Sounds On" : "Sounds Off"; }
        }

        public string ViewSoundStateFile
        {
            get { return (UserSettingsManagement.IsSoundEnabled) ? "View/Resources/SoundsOn.png" : "View/Resources/SoundsOff.png"; }
        }
        
        private bool isRestartEnabled = false;
        public bool IsRestartEnabled
        {
            get { return isRestartEnabled; }
            set { isRestartEnabled = value; Notify("IsRestartEnabled"); }
        }

        private void gameManager_RestartAvaibilityChanged(bool b)
        {
            IsRestartEnabled = b;
        }

        //
        // MENU CLICKS HANDLERS
        //         

        private void miUser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(ProfileRepository.Instance.Server + "/Profile/"));
            e.Handled = true;
        }


        private void miRestart_Click(object sender, RoutedEventArgs e)
        {
            restartFirstPlayer();
        }

        private void miSound_Click(object sender, RoutedEventArgs e)
        {
            UserSettingsManagement.IsSoundEnabled = !UserSettingsManagement.IsSoundEnabled;
            UserSettingsManagement.Save();

            if (gameManager != null)
            {
                gameManager.SetSoundsSettings(UserSettingsManagement.IsSoundEnabled);
            }

            Notify("ViewSoundState");
            Notify("ViewSoundStateFile");
        }


        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog ad = new AboutDialog();
            ad.Closed += new EventHandler(aboutDialog_Closed);
            ad.Owner = this;
            ad.Show();
        }

        void aboutDialog_Closed(object sender, EventArgs e)
        {
            this.Activate();
        }



        private void MenuItem_GameWindow_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(gameManager);
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

        private void MenuItem_PendingGames_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(pendingGamesPane);
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

        private void pendingGamesPane_StateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisibilityOfDockableContents(pendingGamesPane, pendingGamesPane.State);
        }

        private void SetVisibilityOfMenuItems(DocumentPane dp)
        {
            if (dp.Visibility == Visibility.Visible) // the value is set in ConvertBack of AvalonDockVisibilityConverter!!!
            {
                dp.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                dp.Visibility = System.Windows.Visibility.Hidden;
            }

            foreach (DocumentContent dc in dockingManager.Documents)
            {
                if (dp.Visibility == Visibility.Visible)
                {
                    dc.Show();
                }
                else
                {
                    dc.Hide();
                }
            }
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
    }
}
