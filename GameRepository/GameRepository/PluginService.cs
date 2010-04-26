using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sokoban.Lib;
using System.IO;
using Sokoban.Model.PluginInterface;

namespace Sokoban.Model
{
    public class PluginService
    {
        private static SortedDictionary<string, Pair<Type, Assembly>> loadedAssemblies = new SortedDictionary<string, Pair<Type, Assembly>>();
        private static SortedDictionary<string, bool> loadedFiles = new SortedDictionary<string, bool>();
        private static SortedDictionary<string, string> pluginSchemata = new SortedDictionary<string, string>();
        private List<IGamePlugin> runningPlugins = new List<IGamePlugin>(); // only this will be unloaded via Unl

        private IPluginParent pluginHost;


        public PluginService(IPluginParent pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        /// <summary>
        /// Searches the passed Path for Plugins
        /// </summary>
        /// <param name="Path">Directory to search for Plugins in</param>
        public void LoadAllPlugins(string Path)
        {
            if (loadedAssemblies.Count == 0)
            {
                foreach (string fileOn in Directory.GetFiles(Path))
                {
                    FileInfo file = new FileInfo(fileOn);
                    if (file.Extension.Equals(".dll"))
                    {
                        LoadPlugin(fileOn);
                    }
                }
            }
            else
            {
                DebuggerIX.WriteLine("[Plugins]", "LoadPlugins", "Method may be used only when no plugins has been loaded yet.");
            }
        }

        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            runningPlugins.Clear();
        }

        public string GetPluginSchema(string pluginName)
        {
            if (pluginSchemata.ContainsKey(pluginName))
            {
                return pluginSchemata[pluginName];
            }
            else
            {
                if (LoadPlugin(@"D:\Bakalarka\Sokoban\Main\Plugins\Plugin" + pluginName + ".dll"))
                {
                    IGamePlugin gamePlugin = RunPlugin(pluginName);
                    pluginSchemata[pluginName] = gamePlugin.XmlSchema;
                    return pluginSchemata[pluginName];
                }
                else
                {
                    throw new Exception("Plugin `" + pluginName + "' cannot be loaded.");
                }
            }
        }

        public IGamePlugin RunPlugin(string pluginName)
        {
            pluginName = pluginName.ToLower();
            Assembly assembly = loadedAssemblies[pluginName].Second;
            Type type = loadedAssemblies[pluginName].First;
            Type name = assembly.GetType(type.ToString());
            IGamePlugin gamePlugin = (IGamePlugin)Activator.CreateInstance(name, this.pluginHost);

            runningPlugins.Add(gamePlugin);

            return gamePlugin;
        }

        public bool LoadPlugin(string fileName)
        {
            bool succesfullyLoaded = false;
            Assembly pluginAssembly;
            Type pluginType = null;

            if (loadedFiles.ContainsKey(fileName) == false)
            {
                loadedFiles[fileName] = true;                
                pluginAssembly = Assembly.LoadFrom(fileName);

                foreach (Type type in pluginAssembly.GetTypes())
                {
                    if (type.IsPublic && !type.IsAbstract)
                    {
                        Type typeInterface = type.GetInterface("Sokoban.Model.PluginInterface.IGamePlugin", true);

                        if (typeInterface != null)
                        {
                            succesfullyLoaded = true;
                            pluginType = type;
                        }
                    }
                }

                if (succesfullyLoaded)
                {
                    string pluginName = Path.GetFileNameWithoutExtension(fileName).ToLower();
                    string prefix = "plugin";

                    if (pluginName.Length >= prefix.Length && pluginName.Substring(0, prefix.Length) == prefix)
                    {
                        pluginName = pluginName.Substring(prefix.Length);
                    }

                    loadedAssemblies[pluginName] = new Pair<Type, Assembly>(pluginType, pluginAssembly);
                    DebuggerIX.WriteLine("Plugin: " + fileName + " was loaded.");
                }
                else
                {
                    DebuggerIX.WriteLine("File: " + fileName + " could not be loaded.");
                }
            }
            else
            {                
                succesfullyLoaded = true;
            }

            return succesfullyLoaded;
        }

        public void Terminate()
        {
            this.ClosePlugins();            
            runningPlugins = null;
            pluginHost = null;
        }
    }
}
