using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AvalonDock;
using System.Diagnostics;
using Sokoban.View;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for GameDocs.xaml
    /// </summary>
    public partial class Solvers : DocumentPane, INotifyPropertyChanged
    {
        public Solvers()
        {
            InitializeComponent();

            this.DataContext = this;            
        }

        /// <summary>
        /// Shutdown all the solvers
        /// </summary>
        public void Terminate()
        {
            
        }        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion
    }
}
