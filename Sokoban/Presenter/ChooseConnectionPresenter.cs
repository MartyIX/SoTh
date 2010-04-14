using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View.ChooseConnection;
using Sokoban.Model;
using System.Windows;
using Sokoban.View;
using System.Diagnostics;

namespace Sokoban.Presenter
{
    public class ChooseConnectionPresenter : BasePresenter<IChooseConnectionView, ProfileRepository>
    {
        private Window chooseConnectionDialog;
        private ChooseConnectionDebug chooseConnectionDebug;
        private string viewName;


        public ChooseConnectionPresenter(string viewName)
        {
            this.viewName = viewName;
            this.repository = ProfileRepository.Instance;
            this.repository.Initialize();
        }

        public override void InitializeView()
        {

            if (viewName == "form" && (chooseConnectionDialog == null || PresentationSource.FromVisual(chooseConnectionDialog).IsDisposed))
            {
                chooseConnectionDialog = new ChooseConnectionDialog(this);
                this.view = (IChooseConnectionView)chooseConnectionDialog;
                ApplicationRepository.Instance.RegisterMdiWindow(chooseConnectionDialog);
                chooseConnectionDialog.Show();
            }
            else if (viewName == "debug" && chooseConnectionDebug == null)
            {
                chooseConnectionDebug = new ChooseConnectionDebug(this);
                this.view = (IChooseConnectionView)chooseConnectionDebug;
                this.Login();
            }
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
