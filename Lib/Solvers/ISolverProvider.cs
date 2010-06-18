using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public interface ISolverProvider
    {
        int GetMazeWidth();
        int GetMazeHeight();
        string SerializeMaze();
    }
}
