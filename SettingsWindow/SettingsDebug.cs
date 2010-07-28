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

namespace Sokoban.View.Settings
{
    public partial class SettingsDebug : ISettingsView
    {
        private SettingsPresenter presenter;
        private ISettingsRepository model;


        public SettingsDebug(SettingsPresenter presenter, ISettingsRepository model)
        {
            this.presenter = presenter;
            this.model = model;
            model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            /*if (e.PropertyName == "")
            {
                Debug.WriteLine("- " + this.model.LoginMessage);
            }*/
        }

        #region ISettingsView Members

        string IBaseView.Name
        {
            get
            {
                return "SettingsDebug";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ISettingsView.CloseWindow()
        {
            Debug.Write("- Closing Settings view.");
        }

        #endregion


        #region ISettingsView Members

        public bool IsSplashEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}