using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using Sokoban.Lib.Exceptions;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;

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
        public delegate void SolverWorkCompletedDel(object mazeID, uint width, uint height, string maze, string solution);

        // Private Fields
        private Dictionary<string, SolverLibrary> solversDictionary;
        
        //private Dictionary<string, string> solversDictionary;
        private Window parentWindow;
        private string currentSolver = null;
        private ISolverProvider solverProvider;
        private BackgroundWorker solvingThread = new BackgroundWorker();
        private static string solversPath = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="solversPath"></param>
        /// <param name="parentWindow"></param>
        public SolversManager(ISolverProvider solverProvider, Window parentWindow)
        {
            if (solversPath == null) throw new InvalidStateException("Function Initialize must be called before creating an instance of SolversManager.");
            
            solversDictionary = new Dictionary<string, SolverLibrary>();
            this.parentWindow = parentWindow;
            this.solverProvider = solverProvider;
            loadSolverPlugins(solversPath);
           
            solvingThread.DoWork += new DoWorkEventHandler(solvingThread_DoWork);
            solvingThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(solvingThread_RunWorkerCompleted);
        }

        /// <summary>
        /// Must be called before creating an instance of SolversManager!
        /// </summary>
        /// <param name="solversPath"></param>
        public static void Initialize(string _solversPath)
        {
            if (_solversPath.Length == 0)
            {
                throw new InvalidStateException("Path to the solvers must be non-empty string.");
            }
            else if (_solversPath[_solversPath.Length - 1] != '\\' && _solversPath[_solversPath.Length - 1] != '/')
            {
                solversPath = _solversPath + @"\";
            }
            else
            {
                solversPath = _solversPath;
            }                        
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

                    lib.Load();

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
                if (pair.Value != null)
                {
                    pair.Value.StatusCallback += callback;
                }
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
                if (pair.Value != null)
                {
                    pair.Value.StatusCallback -= callback;
                }
            }
        }



        /// <summary>
        /// Solve currrent round (available via SolverProvide in constructor)
        /// </summary>
        /// <returns>If the solution was found then non-empty string is returned otherwise empty one</returns>
        public void SolveRound(SolverWorkCompletedDel solverWorkCompleteDelegate)
        {            
            SolverLibrary lib = this.getLib(currentSolver);            
            uint[] constraints = lib.GetConstraints();

            uint width = solverProvider.GetMazeWidth();
            uint height = solverProvider.GetMazeHeight();
            string maze = solverProvider.SerializeMaze();
            object mazeID = solverProvider.GetIdentifier();
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
                SolverName = currentSolver,
                MazeID = mazeID,
                Height = height,
                Width = width,
                Maze = maze,
                SolverWorkCompleteDel = solverWorkCompleteDelegate
            };

            solvingThread.RunWorkerAsync(param);
        }

        private SolverLibrary getLib(string solverName)
        {
            return solversDictionary[solverName];            
        }



        /// <summary>
        /// BackgroundWorker thread: Method is run in a new thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solvingThread_DoWork(object sender, DoWorkEventArgs e)
        {
            SolvingThreadParameterObject param = e.Argument as SolvingThreadParameterObject;

            // TODO REMOVE
            //System.Threading.Thread.Sleep(5000); // For debugging purposes
           
            SolverLibrary lib = this.getLib(param.SolverName);
            SOKOBAN_PLUGIN_RESULT res = lib.SolveEx(param.Width, param.Height, param.Maze);
            string solution = lib.LastSolution;

            if (res == SOKOBAN_PLUGIN_RESULT.SUCCESS)
            {
                e.Result = new SolvingThreadResultObject 
                {                     
                     SolverName = param.SolverName,
                     MazeID = param.MazeID,
                     Height = param.Height,
                     Width = param.Width,
                     Maze = param.Maze,
                     Solution = solution,
                     SolverWorkCompleteDel = param.SolverWorkCompleteDel,
                } ;
            }
            else
            {
                e.Result = new SolvingThreadResultObject
                {
                    SolverName = param.SolverName,
                    MazeID = param.MazeID,
                    Height = param.Height,
                    Width = param.Width,
                    Maze = param.Maze,
                    Solution = "",
                    SolverWorkCompleteDel = param.SolverWorkCompleteDel,
                };
            }            
        }

        private void solvingThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            SolvingThreadResultObject result = e.Result as SolvingThreadResultObject;
            result.SolverWorkCompleteDel(result.MazeID, result.Width, result.Height, result.Maze, result.Solution);
        }


        public void ShowAbout()
        {
            this.getLib(currentSolver).ShowAbout();
        }

        public void ShowConfigure()
        {
            this.getLib(currentSolver).Configure();
        }

        /// <summary>
        /// The Terminate function allows the caller to terminate the plugin from another thread.
        /// </summary>
        public void TerminateSolver()
        {
            this.getLib(currentSolver).Terminate();
        }

        public void Terminate()
        {            
            // Correctly release all libraries

            foreach (string solverName in Solvers)
            {
                if (solversDictionary[solverName] != null)
                {
                    solversDictionary[solverName].Unload();
                }
            }

            solversDictionary.Clear();
        }

        /// <summary>
        /// Parameter for solving thread
        /// </summary>
        private class SolvingThreadParameterObject
        {
            public string SolverName { get; set; }
            public object MazeID { get; set; }
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
            public string SolverName { get; set; }
            public object MazeID { get; set; }
            public uint Width { get; set; }
            public uint Height { get; set; }
            public string Maze { get; set; }
            public string Solution { get; set; }
            public SolverWorkCompletedDel SolverWorkCompleteDel { get; set; }
        }
    }
}
