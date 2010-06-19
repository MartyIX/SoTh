using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public interface ISolverProvider
    {
        uint GetMazeWidth();
        uint GetMazeHeight();
        string SerializeMaze();
    }
}
