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

                        
            solverPainter = solverProvider as ISolverPainter;

            if (solverPainter == null)
            {
                throw new Exception("Reference `solverProvider' should also implement ISolverPainter.");
            }
        } 


        //
        // Private fields
        //

        private ObservableCollection<MessagesLogItemData> dataGridItemsSource = new ObservableCollection<MessagesLogItemData>();
        private string solverStatus = "No action is in progress";
        private SolversManager solversManager;
        private ObservableCollection<string> solversList = new ObservableCollection<string>();
        
        // Initialized in method Initialize from ISolverProvider -- it's a hack actually because we expect that the object implements
        // ISolverPainter and ISolverProvider at the same time. But we save a parameter..
        private ISolverPainter solverPainter;

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            dataGridItemsSource.Clear();
            SolverStatus = "Solver is running";

            disableStartOfSolver();

            try
            {
                // We specify that we want to get results via method solverWorkCompleted
                solversManager.SolveRound(new SolversManager.SolverWorkCompletedDel(solverWorkCompleted));                
            } 
            catch (SolverException ex) 
            {
                enableStartOfSolver();
                SolverStatus = "Solver cannot finish its work.";
                addItemToMessageLog(0, ex.Message);                
            }                        
        }

        private void disableStartOfSolver()
        {
            butStart.IsEnabled = false;
            cbCurrentSolver.IsEnabled = false;
        }

        private void enableStartOfSolver()
        {
            butStart.IsEnabled = true;
            cbCurrentSolver.IsEnabled = true;
        }


        /// <summary>
        /// Param contains solution. If empty then solver was not able to find a solution
        /// </summary>
        /// <param name="param">Gots string in form of object</param>
        private void solverWorkCompleted(object mazeID, uint width, uint height, string maze, string solution)
        {
            enableStartOfSolver();
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

                if (solverPainter != null)
                {
                    int pos1 = maze.IndexOf('+');
                    int pos2 = maze.IndexOf('@');
                    int sokobanX = 0;
                    int sokobanY = 0;

                    if (pos1 == -1 && pos2 > -1)
                    {
                        pos1 = pos2;
                    }

                    sokobanX = pos1 % (int)width + 1;
                    sokobanY = (pos1 - sokobanX + 1) / (int)width + 1;

                    solverPainter.DrawSolverSolution(mazeID, solution, sokobanX, sokobanY);
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
