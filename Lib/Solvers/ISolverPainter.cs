using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Solvers
{
    public interface ISolverPainter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mazeID">Every maze has its own ID in order not to make a mistake where to to draw the solution</param>
        /// <param name="solution">Characters: l,r,u,d,L,R,U,D in row. 
        ///    Uppercase letters means moving of a box. Format is used as result of a solver)</param>
        /// <param name="sokobanX">One-based x-coordinate of Sokoban</param>
        /// <param name="sokobanY">One-based y-coordinate of Sokoban</param>
        void DrawSolverSolution(object mazeID, string solution, int sokobanX, int sokobanY);

        void DrawSolverSolution(string solution, int sokobanX, int sokobanY);
    }
}
