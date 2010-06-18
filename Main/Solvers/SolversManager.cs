using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;

namespace Sokoban.Solvers
{
    public class SolversManager
    {
        // API
        public List<string> Solvers;

        // Private Fields
        private Dictionary<string, SolverLibrary> solversDictionary;
        //private Dictionary<string, string> solversDictionary;
        private Window parentWindow;
        private string currentSolver = null;
        private ISolverProvider solverProvider;

        public string CurrentSolver
        {
            get { return currentSolver; }
            set { currentSolver = value; }
        }

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
                    //solversDictionary.Add(solverName, file.FullName);
                    solversDictionary.Add(solverName, lib);

                    if (currentSolver == null)
                    {
                        currentSolver = solverName;                        
                    }
                }
            }
        }


        public void SolveRound()
        {
            //SolverLibrary lib = new SolverLibrary(solversDictionary[currentSolver], this.parentWindow);
            SolverLibrary lib = solversDictionary[currentSolver];
            //MessageBox.Show(lib.GetPluginName());
            
            
            lib.SolveEx(19, 17, "##############################################################   ################$  ################  $##############  $ $ ############# # ## ###########   # ## #####  ..## $  $          ..###### ### #@##  ..######     ########################################################################################################");
            //lib.Unload();
        }

        public void Terminate()
        {            
            // Correctly release all libraries
            // TODO

            foreach (string solverName in Solvers)
            {
                solversDictionary[solverName].Unload();
            }

            solversDictionary.Clear();
        }
    }
}
