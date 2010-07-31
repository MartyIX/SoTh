using System;
using Sokoban.Lib;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Collections.Generic;

namespace Sokoban.Model.PluginInterface
{
    public interface IPluginParent
    {
        // For communication between plugins
        void Message(string messageType, object message, IGamePlugin plugin);
        
        // Planning
        void MakePlan(string debug, Int64 when, IGamePlugin who, EventType what, bool force);
        void MakeImmediatePlan(string debug, IGamePlugin who, EventType what);

        // Gamedesk actions
        IGamePlugin GetObstructionOnPosition(int x, int y);

        IEnumerable<IGamePlugin> AllPlugins {get;}
        IGamePlugin[,] GetFixedElements();
        IGamePlugin[,] GetFixedTiles();
        IGameVariant GameVariant { get; }
        
        // Process all events
        void ProcessAllEvents();
        void ProcessAllEvents(double phase);
        void ProcessAllEvents(bool updateTime, double phase);
        bool IsSimulationActive { get; }
        void ResumeSimulation();
        void StopSimulation();

        void PropertyChanged(string name);

        // Sounds
        void RegisterMediaElement(MediaElement me);
    }

    public interface IGamePlugin : IPosition
    {       
        // Plugin identification
        string Name { get; }
        // Plugin description
        string Description { get; }
        // Your name
        string Author { get; }
        // Version of your plugin; e.g. 1.00;
        string Version { get; }        
        // For what version of SoTh application was the plugin created; e.g. 1.00;
        string CreatedForHostVersion { get; }
        // Appearance of your plugin; it may be image or a geometric shape etc. Whatever object that inherits from UIElement
        UIElement UIElement { get; set; }
        // Returns XML schema for what is should be content of tag <YourPluginName></YourPluginName> in round specification
        string XmlSchema { get; }

        // Your initialization goes here; appPath param is without trailing backslash
        void Load(string appPath);
        // In case that you use some unmanaged code and you need to correctly destruct an object, ...
        void Unload();
        // Plugin has to draw itself on gamedesk
        void Draw(Canvas canvas, double squareSize, Int64 time, double phase);        
        // Returns true if initialization was successful
        // settings is content of tag <YourPluginName></YourPluginName> in round specification
        bool ProcessXmlInitialization(string gameVariant, int mazeWidth, int mazeHeight, XmlNode settings);
        // Return false if plugin is not able to process the event, host will take care of the message
        bool ProcessEvent(Int64 time, Event e);
        // For cross-plugin communication; sender is object who sends message
        void MessageReceived(string messageType, object message, IGamePlugin sender);
        int ID { get; set; }        
    }
}
