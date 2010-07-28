using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Configuration;
using System.Windows;
using Sokoban.View.Settings;
using Sokoban.Lib;
using Sokoban.Model;

namespace Sokoban
{
    public partial class MainWindow
    {
        private bool isSoundEnabled = true;
        private bool isSettingsDialogDisplayed = false;

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!isSettingsDialogDisplayed)
            {
                isSettingsDialogDisplayed = true;
                isSoundEnabled = UserSettingsManagement.IsSoundEnabled;
                SettingsPresenter settingsPresenter = ApplicationRepository.Instance.LoadViewSettings();
                settingsPresenter.Closing += new VoidChangeDelegate(modalWindow_Closing);
                settingsPresenter.Closed += new VoidChangeDelegate(settingsPresenter_Closed);

            }
            else
            {
                MessageBoxShow("Settings dialog cannot be opened twice.");
            }
        }

        void settingsPresenter_Closed()
        {
            isSettingsDialogDisplayed = false;
            if (isSoundEnabled != UserSettingsManagement.IsSoundEnabled)
            {
                gameManager.SetSoundsSettings(UserSettingsManagement.IsSoundEnabled);
            }
        }

    }
}
