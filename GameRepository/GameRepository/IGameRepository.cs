using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;
using System.Windows.Input;
using Sokoban.Solvers;

namespace Sokoban.Model.GameDesk
{
    public delegate void GameObjectsLoadedDelegate(List<IGamePlugin> gameObjects);
    public delegate void SetSizeDelegate(int fieldsX, int fieldsY);

    public interface IGameRepository : ISolverProvider
    {
        int FieldsX { get; }
        int FieldsY { get; }
        Int64 Time { get; }
        string RoundName { get; }
        int StepsCount { get; }
        IEnumerable<IGamePlugin> GetGameObjects { get; }
        
        void LoadRoundFromXML(string xml);
        event SetSizeDelegate DeskSizeChanged;
        event GameObjectsLoadedDelegate GameObjectsLoaded;
        void ProcessAllEvents();
        void MoveRequest(Key key);
        void StopMove(Key key);
        IGameRealTime GameRealTime { get; set; }
        PluginService PluginService { get; }

        void Terminate();
    }
}
