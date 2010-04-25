using System;
using Sokoban.Lib;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

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
        void Draw(Canvas canvas, double squareSize, Int64 time, double phase);
        string XmlSchema { get; }
        // Returns true if initialization was successful
        bool ProcessXmlInitialization(XmlNode settings);

        // Return false if plugin is not able to process the event, host will take care of the message
        bool ProcessEvent(Int64 time, Event? e);

        // Plugin's host 
        IPluginParent Parent { get; set; }

        // Plugin's properties
        int ID { get; set; }
        UIElement UIElement { get; set; }


    }
}
