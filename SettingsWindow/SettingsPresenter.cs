using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Sokoban.Presenter;
using Sokoban.Model;
using Sokoban.Lib;

namespace Sokoban.View.Settings
{
    public class SettingsPresenter : BasePresenter<ISettingsView, ISettingsRepository>
    {
        private SettingsDialog settingsDialog;
        private SettingsDebug settingsDebug;
        private string viewName;
        public event VoidChangeDelegate Closed;
        public event VoidChangeDelegate Closing;


        public SettingsPresenter(string viewName, ISettingsRepository model)
        {
            this.viewName = viewName;
            this.repository = model;
            this.repository.Initialize();
        }

        public override void InitializeView(Window window) 
        {
            if (viewName == "form" && (settingsDialog == null || 
                PresentationSource.FromVisual(settingsDialog) == null ||
                PresentationSource.FromVisual(settingsDialog).IsDisposed))
            {
                settingsDialog = new SettingsDialog(this, repository);
                
                this.view = (ISettingsView)settingsDialog;
                settingsDialog.Owner = window;
                settingsDialog.Closing += new System.ComponentModel.CancelEventHandler(settingsDialog_Closing);
                settingsDialog.Closed += new EventHandler(settingsDialog_Closed);
                settingsDialog.Show();
            }
            else if (viewName == "debug" && settingsDebug == null)
            {
                settingsDebug = new SettingsDebug(this, repository);
                
                this.view = (ISettingsView)settingsDebug;
            }
        }

        void settingsDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Closing != null)
            {
                Closing();
            }
        }

        void settingsDialog_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        /// <summary>
        /// View calls this function
        /// </summary>
        public void Save()
        {
            this.repository.Save();
        }        
    }
}
