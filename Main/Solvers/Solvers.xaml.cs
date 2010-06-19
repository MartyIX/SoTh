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
using System.Windows.Controls;
using System.Data;
using Sokoban.Lib.Exceptions;
using System.Windows.Threading;
using System.Threading;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for Solvers.xaml
    /// </summary>
    public partial class Solvers : DockableContent, INotifyPropertyChanged
    {
        //
        // API
        //
        public ObservableCollection<string> SolversList { 
            get { return solversList; }
            set{ solversList = value; Notify("SolversList"); }
        }

        public string CurrentSolver
        {
            get { return (solversManager != null) ? solversManager.CurrentSolver : ""; }
            set
            {
                if (solversManager != null)
                    solversManager.CurrentSolver = value;
                Notify("CurrentSolver");
            }
        }

        // For binding
        public ObservableCollection<MessagesLogItemData> DataGridItemsSource { get { return dataGridItemsSource; } }
        public string SolverStatus 
        { 
            get { return solverStatus; } 
            set { solverStatus = value; Notify("SolverStatus"); } 
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

        public event PropertyChangedEventHandler PropertyChanged;

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
            solversManager = new SolversManager(path, solverProvider, parentWindow);
            // Register callback
            solversManager.RegisterStatusChangeCallback(solverStatusChange);
            SolversList = new ObservableCollection<string>(solversManager.Solvers); // we want to notify
            CurrentSolver = solversManager.CurrentSolver;
        } 


        //
        // Private fields
        //

        private ObservableCollection<MessagesLogItemData> dataGridItemsSource = new ObservableCollection<MessagesLogItemData>();
        private string solverStatus = "No action is in progress";
        private SolversManager solversManager;
        private ObservableCollection<string> solversList = new ObservableCollection<string>();

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            dataGridItemsSource.Clear();
            SolverStatus = "Solver is running";            
            
            try
            {
                // We specify that we want to get results via method solverWorkCompleted
                solversManager.SolveRound(new SolversManager.SolverWorkCompletedDel(solverWorkCompleted));                
            } 
            catch (SolverException ex) 
            {
                SolverStatus = "Solver cannot finish its work.";
                addItemToMessageLog(0, ex.Message);
            }                        
        }

        /// <summary>
        /// Param contains solution. If empty then solver was not able to find a solution
        /// </summary>
        /// <param name="param">Gots string in form of object</param>
        private void solverWorkCompleted(object param)
        {            
            string solution = param as string;
            SolverStatus = "Solver stopped working.";

            if (solution != "")
            {
                // Alert
                addItemToMessageLog(0, "Solution was found and will be presented below:");

                // Display the solution

                for (int i = 0; i < solution.Length; i++)
                {
                    string message;

                    switch (solution[i])
                    {
                        case 'l': message = "Go left"; break;
                        case 'r': message = "Go right"; break;
                        case 'u': message = "Go up"; break;
                        case 'd': message = "Go down"; break;
                        case 'L': message = "Go left and move the box"; break;
                        case 'R': message = "Go right and move the box"; break;
                        case 'U': message = "Go up and move the box"; break;
                        case 'D': message = "Go down and move the box"; break;
                        default:
                            throw new Exception("Unknown meaning of character '" + solution[i].ToString() + "' in solution.");
                    }

                    addItemToMessageLog(i + 1, message);
                }
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            solversManager.TerminateSolver();  
        }        

        /// <summary>
        /// Handler of SolverLibrary.StatusCallback event 
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="message"></param>
        /// <param name="parameter"></param>
        private void solverStatusChange(SolverLibrary.StatusPriority sp, SolverLibrary.AlertCodes ac, string message, object parameter)
        {
            if (sp == SolverLibrary.StatusPriority.FunctionStatusChange)
            {
                if (ac == SolverLibrary.AlertCodes.TerminationInMoment)
                {
                    SolverStatus = "Solver is terminating. It may take a while.";
                }

                addItemToMessageLog(0, message); // 0 means that we don't want to display the moves
            }
        }

        /// <summary>
        /// Must be thread safe!!
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="message"></param>
        private void addItemToMessageLog(int moves, string message)
        {
            string _moves = null;

            if (moves == 0)
            {
                _moves = "";
            }
            else
            {
                _moves = moves.ToString();
            }

            var logItem = new MessagesLogItemData
                {
                    Move = _moves,
                    Plugin = CurrentSolver,
                    Message = message
                };


            // Update UI thread
            if (!MyDataGrid.Dispatcher.CheckAccess())
            {
                MyDataGrid.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        dataGridItemsSource.Add(logItem);
                    }
                ));
            }
            else
            {
                dataGridItemsSource.Add(logItem);
            }            
        }


        #region INotifyPropertyChanged Members


        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            solversManager.ShowConfigure();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            solversManager.ShowAbout();
        }

    }

    public class MessagesLogItemData
    {
        public string Plugin { get; set; }
        public string Move { get; set; }
        public string Message { get; set; }
    }
}
