using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;

namespace Sokoban.Configuration
{
    public class UserSettingsManagement
    {
        public static WindowState WindowState
        {
            get
            {
                return Properties.Settings.Default.WindowState;
            }

            set
            {
                Properties.Settings.Default.WindowState = value;
            }            
        }

        public static bool AreWindowsPropertiesSaved
        {
            get
            {
                return Properties.Settings.Default.AreWindowsPropertiesSaved;
            }

            set
            {
                Properties.Settings.Default.AreWindowsPropertiesSaved = value;
            }            
        }

        public static double WindowTop
        {
            get
            {
                return Properties.Settings.Default.WindowTop;
            }

            set
            {
                Properties.Settings.Default.WindowTop = value;
            }
        }

        public static double WindowHeight
        {
            get
            {
                return Properties.Settings.Default.WindowHeight;
            }

            set
            {
                Properties.Settings.Default.WindowHeight = value;
            }
        }

        public static double WindowWidth
        {
            get
            {
                return Properties.Settings.Default.WindowWidth;
            }

            set
            {
                Properties.Settings.Default.WindowWidth = value;
            }
        }

        public static double WindowLeft
        {
            get
            {
                return Properties.Settings.Default.WindowLeft;
            }

            set
            {
                Properties.Settings.Default.WindowLeft = value;
            }
        }

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