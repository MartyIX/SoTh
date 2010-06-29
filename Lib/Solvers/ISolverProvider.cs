using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Solvers
{    
    public interface ISolverProvider
    {
        uint GetMazeWidth();
        uint GetMazeHeight();
        string SerializeMaze();
        event GameObjectMovedDel SokobanMoved;
        object GetIdentifier();
    }
}
