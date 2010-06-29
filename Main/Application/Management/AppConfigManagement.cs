using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Sokoban.Configuration
{
    public class AppConfigManagement
    {
        public static bool EditKeyPair(string key, string value)
        {
            bool success = true;
            
            // Open App.Config of executable
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;
           
            // update SaveBeforeExit
            settings[key].Value = value;
                        
            if (!config.AppSettings.SectionInformation.IsLocked)
            {
                //save the file
                config.Save(ConfigurationSaveMode.Modified);
                Debug.WriteLine("** Settings updated.");
            }
            else
            {
                Debug.WriteLine("** Could not update, section is locked.");
                success = false;
            }

            //reload the section you modified
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

            return success;
        }


        public static bool AddKeyPair(string key, string value)
        {
            bool success = true;

            // Open App.Config of executable
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;

            // Add an Application Setting.
            config.AppSettings.Settings.Add(key, value);

            if (!config.AppSettings.SectionInformation.IsLocked)
            {
                //save the file
                config.Save(ConfigurationSaveMode.Modified);
                Debug.WriteLine("** Settings updated.");
            }
            else
            {
                Debug.WriteLine("** Could not update, section is locked.");
                success = false;
            }            

            //reload the section you modified
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

            return success;
        }

        public static NameValueCollection Settings
        {
            get
            {
                return ConfigurationManager.AppSettings;
            }
        }
    
        
        public static void DebugConfig()
        {
            // For read access you do not need to call OpenExeConfiguraton
            foreach (string key in ConfigurationManager.AppSettings)
            {
                string value = ConfigurationManager.AppSettings[key];
                Debug.WriteLine("Key: " + key + ", Value: " + value);
            }
        }
    }
    
    class ServerElement : System.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }
    }

    class ServerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).Name;
        }
    }

    class ServerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers
        {
            get 
            {                
                return (ServerElementCollection)this["servers"];
            }            
        }
    }
}