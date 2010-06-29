using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Sokoban.Configuration;

namespace Sokoban.Model.Settings
{
    public class SettingsRepository : ISettingsRepository
    {
        /* Singleton: private instance, private constructor and Instance method */
        private static readonly SettingsRepository instance = new SettingsRepository();
        private SettingsRepository() { }

        public static SettingsRepository Instance
        {
            get
            {
                return instance;
            }
        }

        #region ISettingsRepository Members

        public NameValueCollection Settings
        {
            get { return AppConfigManagement.Settings; }
        }

        public void Save(string key, string value)
        {
            AppConfigManagement.EditKeyPair(key, value);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IBaseRepository Members

        public void Initialize()
        {
        }

        #endregion
    }
}
