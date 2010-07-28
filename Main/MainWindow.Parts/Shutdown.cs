﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Configuration;
using System.Windows;
using Sokoban.Lib;
using System.IO;

namespace Sokoban
{
    public partial class MainWindow
    {
        /// <summary>
        /// Correctly terminates everything in the main window that needs it
        /// </summary>
        private void Terminate()
        {
            //
            // Save layout if enabled
            //
            if (UserSettingsManagement.IsSavingAppLayoutEnabled)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                dockingManager.SaveLayout(sw);
                UserSettingsManagement.WindowLayout = sb.ToString();
                UserSettingsManagement.Save();
            }

            // 
            // End of: Save layout if enabled
            //


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
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Terminate();
            DebuggerIX.Close();
        }

    }
}
