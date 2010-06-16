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
using Sokoban.Model;

namespace Sokoban.View
{
    public partial class ChooseConnectionDebug : IChooseConnectionView
    {
        private ChooseConnectionPresenter presenter;
        private IProfileRepository model;

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

        string IChooseConnectionView.SelectedURL
        {
            get
            {
                return ApplicationRepository.Instance.appParams.credentials["server"];
            }
        }

        string IChooseConnectionView.Username
        {
            get { return ApplicationRepository.Instance.appParams.credentials["username"]; }
        }

        string IChooseConnectionView.Password
        {
            get { return ApplicationRepository.Instance.appParams.credentials["password"]; }
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