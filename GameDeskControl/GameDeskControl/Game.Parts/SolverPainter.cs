using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Solvers;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game : ISolverPainter
    {
        private SolverPainter solverPainter;

        private IMovableElement solversSokobanRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution">Characters: l,r,u,d,L,R,U,D in row. 
        ///    Uppercase letters means moving of a box. Format is used as result of a solver)</param>
        /// <param name="sokobanX">One-based x-coordinate of Sokoban</param>
        /// <param name="sokobanY">One-based y-coordinate of Sokoban</param>
        public void DrawSolverSolution(string solution, int sokobanX, int sokobanY)
        {
            if (solversSokobanRef == null)
            {

                foreach (IGamePlugin gp in this.GameRepository.GetGameObjects)
                {
                    if (gp.Name == "Sokoban")
                    {
                        IMovableElement me = gp as IMovableElement;

                        // Register event only when it is needed; when to unregister??
                        if (SokobanMoved == null)
                        {
                            if (me != null)
                            {
                                me.ElementMoved += new GameObjectMovedDel(sokoban_ElementMoved);
                            }
                        }

                        solversSokobanRef = me;

                        break;
                    }
                }

                if (solversSokobanRef == null)
                {
                    MessageBoxShow("Sokoban object was not found in this game variant.");
                }
            }

            // only for game variants with sokoban
            if (solversSokobanRef != null)
            {
                if (solversSokobanRef.PosX != sokobanX || solversSokobanRef.PosY != sokobanY)
                {
                    MessageBoxShow("Sokoban changed position during the time the solver was finding a solution.\nThe solution is therefore invalid.");
                }
                else
                {
                    // Remove old painter
                    this.removeOldPainter();

                    // New painter
                    solverPainter = new SolverPainter(control.Canvas, sokobanX, sokobanY, control.FieldSize, solution);
                    solverPainter.Draw();
                    SokobanMoved += solverPainter.Update;
                }
            }
        }

        private void removeOldPainter()
        {
            if (solverPainter != null)
            {
                SokobanMoved -= solverPainter.Update;
                solverPainter.Terminate();                
            }
        }

        public void DrawSolverSolution(object mazeID, string solution, int sokobanX, int sokobanY)
        {
            if (mazeID != this)
            {
                throw new Exception("Wrong reference to maze encountered.");
            }

            DrawSolverSolution(solution, sokobanX, sokobanY);
        }

        void sokoban_ElementMoved(int newX, int newY, char direction)
        {
            if (SokobanMoved != null)
            {
                SokobanMoved(newX, newY, direction);
            }
        }
    }
}
