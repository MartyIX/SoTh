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

        public object this[string s]
        {
            get
            {
                if (s == "IsSplashEnabled")
                {
                    return UserSettingsManagement.IsSplashEnabled;
                }
                else if (s == "IsSoundEnabled")
                {
                    return UserSettingsManagement.IsSoundEnabled;
                }
                else if (s == "IsSavingAppLayoutEnabled")
                {
                    return UserSettingsManagement.IsSavingAppLayoutEnabled;
                }
                else
                {
                    throw new Exception("Property `" + s + "' is not in user-settings.");
                }
            }

            set 
            {            
                if (s == "IsSplashEnabled")
                {
                    UserSettingsManagement.IsSplashEnabled = (bool)value;
                }
                else if (s == "IsSoundEnabled")
                {
                    UserSettingsManagement.IsSoundEnabled = (bool)value;                
                }
                else if (s == "IsSavingAppLayoutEnabled")
                {
                    UserSettingsManagement.IsSavingAppLayoutEnabled = (bool)value;                
                } 
                else 
                {
                    throw new Exception("Property `" + s + "' is not in user-settings.");
                }
            }
        }


        public void Save()
        {
            UserSettingsManagement.Save();
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
