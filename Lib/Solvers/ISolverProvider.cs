using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Solvers
{
    public enum SolverProviderIdentifierType
    {
        DocumentPaneInstance,
        RoundsID
    }

    public interface ISolverProvider
    {
        uint GetMazeWidth();
        uint GetMazeHeight();
        string SerializeMaze();
        string MovementsSoFar { get; }
        event GameObjectMovedDel SokobanMoved;
        object GetIdentifier(SolverProviderIdentifierType id);
    }
}
