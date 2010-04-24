using System;
using Sokoban.Lib;

namespace Sokoban.Model.PluginInterface
{
    public interface IPluginParent
    {
        // For undocumented/dev features :-))
        void Message(string message, IGamePlugin plugin);
        
        // Planning
        void MakePlan(string debug, Int64 when, IGamePlugin who, EventType what);
        void MakeImmediatePlan(string debug, IGamePlugin who, EventType what);

        // Process all events
        void ProcessAllEvents();
        void ProcessAllEvents(double phase);
        void ProcessAllEvents(bool updateTime, double phase);        
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

        // Return false if plugin is not able to process the event, host will take care of the message
        bool ProcessEvent(Int64 time, Event? e);

        // Plugin's host 
        IPluginParent Parent { get; set; }

        // Plugin's properties
        int ID { get; set; }
    }
}
