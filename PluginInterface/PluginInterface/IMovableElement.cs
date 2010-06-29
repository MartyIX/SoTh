using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Model.PluginInterface
{
    public interface IMovableElement : IPosition
    {
        int Speed { get; set; }
        void MakeMove(MovementDirection whereTo);
        Int64 MovementStartTime {get; set;}
        double MovementStartTimePhase { get; set; }
        Int64 MovementEndTime { get; set;}
        EventType LastMovementEvent { get; set; }
        int MovementEventsInBuffer { get; set; }
        Int64 TimeMovementEnds { get; set; }
        event GameObjectMovedDel ElementMoved;
    }
}
