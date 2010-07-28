using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.PluginInterface
{
    public interface IGameVariant
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
        
        // Your initialization goes here; appPath param is without trailing backslash
        void Load(string appPath);
        // In case that you use some unmanaged code and you need to correctly destruct an object, ...
        void Unload();

        void CheckRound(Int64 time);
        void CheckRound(Int64 time, string messageType, object data);
    }
}
