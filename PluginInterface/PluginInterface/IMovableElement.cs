using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Model.PluginInterface
{
    public interface IMovableElement : IPosition
    {
        int Speed { get; set; } // value "0" means that Element that pushes this Element will provide the speed
        void MakeMove(MovementDirection whereTo);
        Int64 MovementStartTime {get; set;}
        double MovementStartTimePhase { get; set; }
        Int64 MovementEndTime { get; set;}
        EventType LastMovementEvent { get; set; }
        int MovementEventsInBuffer { get; set; }
        Int64 TimeWholeMovementEnds { get; set; }
        event GameObjectMovedDel ElementMoved;
        int ObstructionLevel(IGamePlugin asker); // e.g., 5 is for Box, 10 is for Sokoban
    }
}
