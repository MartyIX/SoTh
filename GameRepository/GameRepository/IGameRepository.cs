using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;
using System.Windows.Input;
using Sokoban.Solvers;
using System.Windows.Controls;

namespace Sokoban.Model.GameDesk
{
    public delegate void GameObjectsLoadedDelegate(List<IGamePlugin> gameObjects);
    public delegate void SetSizeDelegate(int fieldsX, int fieldsY);
    public delegate void NewMediaElementDelegate(MediaElement me);

    public interface IGameRepository : ISolverProvider
    {
        int FieldsX { get; }
        int FieldsY { get; }
        Int64 Time { get; set; }
        string RoundName { get; }
        int StepsCount { get; }
        IEnumerable<IGamePlugin> GetGameObjects { get; }

        int EventsInCalendar { get; }
        void LoadRoundFromXML(string xml);
        void ProcessAllEvents();
        bool MoveRequest(Key key, double phase);
        bool StopMove(Key key, double phase);
        IGameRealTime GameRealTime { get; set; }
        PluginService PluginService { get; }
        void MakePlan(int ID, Int64 when, EventType what);


        event SetSizeDelegate DeskSizeChanged;
        event GameObjectsLoadedDelegate GameObjectsLoaded;
        event VoidChangeDelegate GameStarted;
        event NewMediaElementDelegate MediaElementAdded;
        event GameChangeDelegate GameChanged;

        void Terminate();
    }
}
