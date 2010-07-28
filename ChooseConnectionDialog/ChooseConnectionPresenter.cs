using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Sokoban.Presenter;
using Sokoban.Model;
using Sokoban.Lib;

namespace Sokoban.View.ChooseConnection
{
    public class ChooseConnectionPresenter : BasePresenter<IChooseConnectionView, IProfileRepository>
    {
        private ChooseConnectionDialog chooseConnectionDialog;
        private ChooseConnectionDebug chooseConnectionDebug;
        private string viewName;
        public Window mainApp = null;
        public bool isConnected = false;
        public event VoidChangeDelegate Closing;


        public ChooseConnectionPresenter(string viewName, IProfileRepository model)
        {
            this.viewName = viewName;
            this.repository = model;
            this.repository.Initialize();
        }

        public override void InitializeView(Window window) 
        {
            mainApp = window;
            InitializeView_preFill(window, "", "", "");
        }

        public void InitializeView_preFill(Window mainApp, string server, string username, string password)
        {
            this.mainApp = mainApp;

            if (viewName == "form" && (chooseConnectionDialog == null || PresentationSource.FromVisual(chooseConnectionDialog).IsDisposed))
            {
                chooseConnectionDialog = new ChooseConnectionDialog(this, repository);
                chooseConnectionDialog.SelectedURL = server;
                chooseConnectionDialog.Username = username;
                chooseConnectionDialog.Password = password;

                this.view = (IChooseConnectionView)chooseConnectionDialog;
                chooseConnectionDialog.Owner = mainApp;
                chooseConnectionDialog.Closed += new EventHandler(chooseConnectionDialog_Closed);
                chooseConnectionDialog.Show();
                
            }
            else if (viewName == "debug" && chooseConnectionDebug == null)
            {
                chooseConnectionDebug = new ChooseConnectionDebug(this, repository);
                chooseConnectionDebug.SelectedURL = server;
                chooseConnectionDebug.Username = username;
                chooseConnectionDebug.Password = password;

                this.view = (IChooseConnectionView)chooseConnectionDebug;
                this.Login();
                dialogClosing();
            }
        }

        void chooseConnectionDialog_Closed(object sender, EventArgs e)
        {
            mainApp.Activate();
            dialogClosing();
        }

        private void dialogClosing()
        {            
            if (Closing != null)
            {
                Closing();
            }
        }

        public void Login()
        {
            if (repository.TryLogin(this.view.SelectedURL, this.view.Username, this.view.Password))
            {
                isConnected = true;
                // Login was successful
                Debug.WriteLine("- Login successful");
                this.view.CloseWindow();
            }
        }

        public void SkipLogin()
        {
            repository.SkipLogin();
            this.view.CloseWindow();
        }
    }
}
