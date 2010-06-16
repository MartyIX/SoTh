using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Sokoban.Presenter;
using Sokoban.Model;

namespace Sokoban.View.ChooseConnection
{
    public class ChooseConnectionPresenter : BasePresenter<IChooseConnectionView, IProfileRepository>
    {
        private Window chooseConnectionDialog;
        //private ChooseConnectionDebug chooseConnectionDebug;
        private string viewName;


        public ChooseConnectionPresenter(string viewName, IProfileRepository model)
        {
            this.viewName = viewName;
            this.repository = model;
            this.repository.Initialize();
        }

        public override void InitializeView(Window mainApp)
        {

            if (viewName == "form" && (chooseConnectionDialog == null || PresentationSource.FromVisual(chooseConnectionDialog).IsDisposed))
            {
                chooseConnectionDialog = new ChooseConnectionDialog(this, repository);
                this.view = (IChooseConnectionView)chooseConnectionDialog;
                chooseConnectionDialog.Owner = mainApp;
                chooseConnectionDialog.Show();
            }
            //else if (viewName == "debug" && chooseConnectionDebug == null)
            //{
                /*chooseConnectionDebug = new ChooseConnectionDebug(this, repository);
                this.view = (IChooseConnectionView)chooseConnectionDebug;
                this.Login();*/
            //}
        }

        public void Login()
        {
            if (repository.TryLogin(this.view.SelectedURL, this.view.Username, this.view.Password))
            {
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
