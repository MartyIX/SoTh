using System;
using Sokoban.Lib;

namespace Sokoban.Model.PluginInterface
{
    public interface IPluginParent
    {
        void Message(string message, IGamePlugin plugin);
        void MakePlan(string debug, Int64 when, IGamePlugin who, EventType what);
    }

    public interface IGamePlugin
    {       
        // Plugin identification
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        string CreatedForHostVersion { get; }

        // Plugin main actions
        void Load();
        void Unload();

        // Plugin's host 
        IPluginParent Parent { get; set; }

        // Plugin's properties

    }
}
