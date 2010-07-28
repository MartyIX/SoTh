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

        public static bool IsSoundEnabled
        {
            get
            {
                return Properties.Settings.Default.IsSoundEnabled;
            }

            set
            {
                Properties.Settings.Default.IsSoundEnabled = value;
            }
        }

        public static bool IsSavingAppLayoutEnabled
        {
            get
            {
                return Properties.Settings.Default.IsSavingWindowLayoutEnabled;
            }

            set
            {
                Properties.Settings.Default.IsSavingWindowLayoutEnabled = value;
            }
        }

        public static string WindowLayout
        {
            get
            {
                return Properties.Settings.Default.WindowLayout;
            }

            set
            {
                Properties.Settings.Default.WindowLayout = value;
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