using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using Sokoban.Lib.Exceptions;
using System.ComponentModel;

namespace Sokoban.Solvers
{
    public class SolversManager
    {
        // API
        public List<string> Solvers;
        public string CurrentSolver
        {
            get { return currentSolver; }
            set { currentSolver = value; }
        }

        /// <summary>
        /// Param contains result
        /// </summary>
        /// <param name="param"></param>
        public delegate void SolverWorkCompletedDel(object param);

        // Private Fields
        private Dictionary<string, SolverLibrary> solversDictionary;
        //private Dictionary<string, string> solversDictionary;
        private Window parentWindow;
        private string currentSolver = null;
        private ISolverProvider solverProvider;
        private BackgroundWorker solvingThread = new BackgroundWorker();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="solversPath"></param>
        /// <param name="parentWindow"></param>
        public SolversManager(string solversPath, ISolverProvider solverProvider, Window parentWindow)
        {
            solversDictionary = new Dictionary<string, SolverLibrary>();
            //solversDictionary = new Dictionary<string, string>();
            this.parentWindow = parentWindow;
            this.solverProvider = solverProvider;
            loadSolverPlugins(solversPath);

            solvingThread.DoWork += new DoWorkEventHandler(solvingThread_DoWork);
            solvingThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(solvingThread_RunWorkerCompleted);
        }

        /// <summary>
        /// Find solvers in given @path and save them to the solvers collection
        /// </summary>
        /// <param name="path">Path where the solvers are stored (as dll libraries)</param>
        private void loadSolverPlugins(string path)
        {
            Solvers = new List<string>();

            foreach (string fileOn in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(fileOn);

                if (file.Extension.Equals(".dll"))
                {
                    string solverName = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
                    SolverLibrary lib = new SolverLibrary(file.FullName, this.parentWindow);                    
                    Solvers.Add(solverName);
                    solversDictionary.Add(solverName, lib);

                    if (currentSolver == null)
                    {
                        currentSolver = solverName;                        
                    }
                }
            }
        }

        /// <summary>
        /// Register a callback for all Solvers (plugins). Method may be called whenever after creation of SolversManager object
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterStatusChangeCallback(SolverLibrary.StatusCallbackDel callback)
        {
            foreach (KeyValuePair<string, SolverLibrary> pair in solversDictionary)
            {
                pair.Value.StatusCallback += callback;
            }
        }

        /// <summary>
        /// Unregister the callback fror all Solvers (plugins). Method may be called whenever after creation of SolversManager object
        /// </summary>
        /// <param name="callback"></param>
        public void UnRegisterStatusChangeCallback(SolverLibrary.StatusCallbackDel callback)
        {
            foreach (KeyValuePair<string, SolverLibrary> pair in solversDictionary)
            {
                pair.Value.StatusCallback -= callback;
            }
        }

        /// <summary>
        /// Solve currrent round (available via SolverProvide in constructor)
        /// </summary>
        /// <returns>If the solution was found then non-empty string is returned otherwise empty one</returns>
        public void SolveRound(SolverWorkCompletedDel solverWorkCompleteDelegate)
        {
            SolverLibrary lib = solversDictionary[currentSolver];

            uint[] constraints = lib.GetConstraints();

            uint width = solverProvider.GetMazeWidth();
            uint height = solverProvider.GetMazeHeight();
            string maze = solverProvider.SerializeMaze();
            uint boxesNo = 0;
            // Count boxes
            for (int i = 0; i < maze.Length; i++)
            {
                if (maze[i] == '*' || maze[i] == '$') boxesNo++;
            }            

            if (width > constraints[0] && constraints[0] != 0) // 0 = unlimited
            {
                throw new SolverException("Maze width is " + width.ToString() +". Solver can solve mazes with width up to " + 
                    constraints[0].ToString());
            }
            else if (height > constraints[1] && constraints[1] != 0) // 0 = unlimited
            {
                throw new SolverException("Maze height is " + width.ToString() + ". Solver can solve mazes with height up to " +
                    constraints[1].ToString());
            }
            else if (boxesNo > constraints[2] && constraints[2] != 0) // 0 = unlimited
            {
                throw new SolverException("Maze contains " + width.ToString() + " boxes. Solver can solve mazes with boxes up to " +
                    constraints[2].ToString());
            }
            else if (solvingThread.IsBusy == true)
            {
                throw new SolverException("Your last request was not processed yet. Please wait.");
            }

            SolvingThreadParameterObject param = new SolvingThreadParameterObject
            {
                Lib = lib,
                Height = height,
                Width = width,
                Maze = maze,
                SolverWorkCompleteDel = solverWorkCompleteDelegate
            };

            solvingThread.RunWorkerAsync(param);                          
        }

        /// <summary>
        /// BackgroundWorker thread: Method is run in a new thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solvingThread_DoWork(object sender, DoWorkEventArgs e)
        {
            SolvingThreadParameterObject param = e.Argument as SolvingThreadParameterObject;

            if (param.Lib.SolveEx(param.Width, param.Height, param.Maze) == SOKOBAN_PLUGIN_RESULT.SUCCESS)
            {
                e.Result = new SolvingThreadResultObject {
                 Solution = param.Lib.LastSolution,
                 SolverWorkCompleteDel = param.SolverWorkCompleteDel,
                } ;
            }
            else
            {
                e.Result = new SolvingThreadResultObject
                {
                    Solution = "",
                    SolverWorkCompleteDel = param.SolverWorkCompleteDel,
                };
            }            
        }

        private void solvingThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SolvingThreadResultObject result = e.Result as SolvingThreadResultObject;
            result.SolverWorkCompleteDel(result.Solution);
        }


        public void ShowAbout()
        {
            SolverLibrary lib = solversDictionary[currentSolver];
            lib.ShowAbout();
        }

        public void ShowConfigure()
        {
            SolverLibrary lib = solversDictionary[currentSolver];
            lib.Configure();
        }

        /// <summary>
        /// The Terminate function allows the caller to terminate the plugin from another thread.
        /// </summary>
        public void TerminateSolver()
        {
            SolverLibrary lib = solversDictionary[currentSolver];
            lib.Terminate();
        }

        public void Terminate()
        {            
            // Correctly release all libraries

            foreach (string solverName in Solvers)
            {
                solversDictionary[solverName].Unload();
            }

            solversDictionary.Clear();
        }

        /// <summary>
        /// Parameter for solving thread
        /// </summary>
        private class SolvingThreadParameterObject
        {
            public SolverLibrary Lib { get; set; }
            public uint Width { get; set; }
            public uint Height { get; set; }
            public string Maze { get; set; }
            public SolverWorkCompletedDel SolverWorkCompleteDel { get; set; }
        }


        /// <summary>
        /// Result of solving thread
        /// </summary>
        private class SolvingThreadResultObject
        {            
            public string Solution { get; set; }
            public SolverWorkCompletedDel SolverWorkCompleteDel { get; set; }
        }
    }
}
