using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Sokoban.Presenter;
using Sokoban.Model;
using System.Collections;
using System.Diagnostics;
using System.Windows.Data;

namespace Sokoban.View.ChooseConnection
{
    public partial class ChooseConnectionDebug : IChooseConnectionView
    {
        private ChooseConnectionPresenter presenter;
        private IProfileRepository model;
        private string server;
        private string username;
        private string password;


        public ChooseConnectionDebug(ChooseConnectionPresenter presenter, IProfileRepository model)
        {
            this.presenter = presenter;
            this.model = model;
            model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LoginMessage")
            {
                Debug.WriteLine("- " + this.model.LoginMessage);
            }
        }

        #region IChooseConnectionView Members

        public string SelectedURL
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        string IBaseView.Name
        {
            get
            {
                return "ChooseConnectionDebug";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void IChooseConnectionView.CloseWindow()
        {
            Debug.Write("- Closing ChooseConnection view.");
        }

        #endregion

    }
}