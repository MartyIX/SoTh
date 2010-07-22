using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Sokoban.Lib;
using System.IO;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib.Exceptions;
using System.Security;

namespace Sokoban.Model
{
    public class PluginService
    {
        private static SortedDictionary<string, Pair<Type, Assembly>> loadedAssemblies = new SortedDictionary<string, Pair<Type, Assembly>>();
        private static SortedDictionary<string, bool> loadedFiles = new SortedDictionary<string, bool>();
        private static SortedDictionary<string, string> pluginSchemata = new SortedDictionary<string, string>();
        private List<IGamePlugin> runningPlugins = new List<IGamePlugin>(); // only this will be unloaded via Unl

        private IPluginParent pluginHost;

        /// <summary>
        /// Path that must end with trailing backslash (i.e., "\")
        /// </summary>
        private static string pluginsPath = null;

        public PluginService(IPluginParent pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        /// <summary>
        /// Must be called before usage of the class
        /// </summary>
        /// <param name="_pluginsPath"></param>
        public static void Initialize(string _pluginsPath)
        {
            if (_pluginsPath.Length == 0)
            {
                throw new InvalidStateException("Path to the plugins must be non-empty string.");
            }
            else if (_pluginsPath[_pluginsPath.Length - 1] != '\\' || _pluginsPath[_pluginsPath.Length - 1] != '/')
            {
                pluginsPath = _pluginsPath + @"\";
            }
            else
            {
                pluginsPath = _pluginsPath;            
            }            
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
            foreach (IGamePlugin p in runningPlugins)
            {
                p.Unload();
            }

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
                if (pluginsPath == null) throw new InvalidStateException("PluginService class was not initialized.");

                LoadPlugin(this.getPluginPath(pluginName));                
                IGamePlugin gamePlugin = RunPlugin(pluginName);
                pluginSchemata[pluginName] = gamePlugin.XmlSchema;
                return pluginSchemata[pluginName];
            }
        }

        public IGamePlugin RunPlugin(string pluginName)
        {
            pluginName = pluginName.ToLower();
                        
            if (loadedAssemblies.ContainsKey(pluginName) == false)
            {
                this.LoadPluginByName(pluginName);
            }

            Assembly assembly = loadedAssemblies[pluginName].Second;
            Type type = loadedAssemblies[pluginName].First;
            Type name = assembly.GetType(type.ToString());
            IGamePlugin gamePlugin = (IGamePlugin)Activator.CreateInstance(name, this.pluginHost);

            runningPlugins.Add(gamePlugin);

            return gamePlugin;
        }

        private string getPluginPath(string pluginName)
        {
            return pluginsPath + "Plugin" + pluginName + ".dll";
        }

        public void LoadPluginByName(string pluginName)
        {
            LoadPlugin(this.getPluginPath(pluginName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Absolute path to the plugin</param>
        /// <returns></returns>
        public void LoadPlugin(string fileName)
        {
            bool succesfullyLoaded = false;
            Assembly pluginAssembly = null;
            Type pluginType = null;

            if (loadedFiles.ContainsKey(fileName) == false)
            {
                loadedFiles[fileName] = true;

                try
                {
                    pluginAssembly = Assembly.LoadFrom(fileName);
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        if (type.IsPublic && !type.IsAbstract)
                        {
                            if (type.GetInterface("Sokoban.Model.PluginInterface.IGamePlugin", true) != null)
                            {
                                succesfullyLoaded = true;
                                pluginType = type;
                            }
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    succesfullyLoaded = false;
                    DebuggerIX.WriteLine("Plugin: " + fileName + " - file not found exception: " + e.Message);
                }
                catch (SecurityException e)
                {
                    succesfullyLoaded = false;
                    DebuggerIX.WriteLine("Plugin: " + fileName + " - security exception: " + e.Message);
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


            if (succesfullyLoaded == false)
            {
                throw new PluginLoadFailedException("Cannot load plugin `" + fileName + "' cannot be loaded.");
            }
        }

        public void Terminate()
        {
            this.ClosePlugins();            
            runningPlugins = null;
            pluginHost = null;
        }
    }
}
