using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;

namespace Sokoban.Configuration
{
    public class UserSettingsManagement
    {
        public static bool IsSplashEnabled
        {
            get 
            {
                return Properties.Settings.Default.IsSplashEnabled;
            } 

            set 
            {
                Properties.Settings.Default.IsSplashEnabled = value;                
            }            
        }

        public static string ConsoleCommandPrefix
        {
            get
            {
                return Properties.Settings.Default.ConsoleCommandPrefix;
            }

            set
            {
                Properties.Settings.Default.ConsoleCommandPrefix = value;
            }
        }

        public static string ConsoleInitialText
        {
            get
            {
                return Properties.Settings.Default.ConsoleInitialText;
            }
        }


        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }

}