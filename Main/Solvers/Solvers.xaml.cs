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
using Sokoban.Solvers;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for Solvers.xaml
    /// </summary>
    public partial class Solvers : DockableContent, INotifyPropertyChanged
    {
        // API
        public ObservableCollection<string> SolversList { 
            get { return solversList; }
            set{ solversList = value; Notify("SolversList"); }
        }

        public string CurrentSolver
        {
            get { return (solversManager != null) ? solversManager.CurrentSolver : ""; }
            set { if (solversManager != null) 
                     solversManager.CurrentSolver = value; 
                  Notify("CurrentSolver"); }
        }

        // Private fields
        private SolversManager solversManager;
        private ObservableCollection<string> solversList = new ObservableCollection<string>();

        public Solvers()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Absolute path to the directory with solvers</param>
        /// <param name="parentWindow">We need to display dialogs and the dialogs need to be bound to a window; main window reference is expected</param>
        public void Initialize(string path, ISolverProvider solverProvider, Window parentWindow)
        {
            solversManager = new SolversManager(path, parentWindow);
            SolversList = new ObservableCollection<string>(solversManager.Solvers); // we want to notify
            CurrentSolver = solversManager.CurrentSolver;
        } 


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            solversManager.SolveRound();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
        
        /// <summary>
        /// Shutdown all the solvers
        /// </summary>
        public void Terminate()
        {
            if (solversManager != null)
            {
                solversManager.Terminate();
            }
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
