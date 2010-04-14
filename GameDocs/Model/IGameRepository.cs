using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model.GameDesk
{
    interface IGameRepository
    {
        int FieldsX { get; }
        int FieldsY { get; }
        Int64 Time { get; }
        IEnumerable<GameObject> GetGameObjects { get; }
        
        void LoadRoundFromXML(string xml);
        event SetSizeDelegate DeskSizeChanged;
        event GameObjectsLoadedDelegate GameObjectsLoaded;
        void ProcessAllEvents();
        void MoveRequest(EventType ev);
        void StopMove();
        Game Game { get; set; }
    }
}
