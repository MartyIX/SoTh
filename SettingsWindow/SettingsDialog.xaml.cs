using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sokoban.Model;
using Sokoban.View.Settings;

namespace Sokoban.View
{           
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsDialog : Window, ISettingsView
    {
        private ISettingsRepository model;
        public SettingsPresenter presenter;

        public SettingsDialog(SettingsPresenter presenter, ISettingsRepository model)
        {
            InitializeComponent();
            DataContext = this;

            this.presenter = presenter;
            this.model = model;
        }

        #region ISettingsView Members

        public void CloseWindow()
        {
            this.Close();
        }

        public bool IsSplashEnabled
        {
            get
            {
                return (bool)this.model["IsSplashEnabled"];
            }
            set
            {
                this.model["IsSplashEnabled"] = value;
            }
        }

        public bool IsSavingAppLayoutEnabled
        {
            get
            {
                return (bool)this.model["IsSavingAppLayoutEnabled"];
            }
            set
            {
                this.model["IsSavingAppLayoutEnabled"] = value;
            }
        }


        public bool IsSoundEnabled
        {
            get
            {
                return (bool)this.model["IsSoundEnabled"];
            }
            set
            {
                this.model["IsSoundEnabled"] = value;
            }
        }


        #endregion

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.presenter.Save();
            this.CloseWindow();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
        }


    }
}
