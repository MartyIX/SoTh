using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Application
{
    public class ApplicationParameters
    {
        public Dictionary<string, string> views = null;
        public Dictionary<string, string> credentials = null;
        public Dictionary<string, string> NetworkGame = null;
        public string DebuggerPath = null;
        public Dictionary<DebuggerTag, bool> DebuggerIX_Tags = null;

        public List<string> OpenViewsOnStart = null;

        public string getView(string viewType)
        {
            if (views == null)
            {
                return "form";
            }
            else
            {
                if (views.ContainsKey(viewType))
                {
                    return views[viewType];
                }
                else
                {
                    return "form";
                }
            }
        }

        private void setView(string viewName, string view)
        {
            if (views == null)
            {
                views = new Dictionary<string, string>();
            }

            if (views.ContainsKey(viewName))
            {
                views[viewName] = view;
            }
            else
            {
                views.Add(viewName, view);
            }
        }

        public void ProcessParameters(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "--enable-debugging")
                {
                    if (i + 1 < args.Length)
                    {
                        DebuggerPath = args[i + 1];
                    }
                    else
                    {
                        throw new ApplicationParametersException("Missing path for debugging");
                    }
                }
                else if (args[i].ToLower() == "--auto-connect")
                {
                    // Syntax: --auto-connect <username> <pass>

                    if (i + 3 < args.Length)
                    {
                        if (credentials == null)
                        {
                            credentials = new Dictionary<string, string>();
                            credentials["server"] = args[i + 1];
                            credentials["username"] = args[i + 2];
                            credentials["password"] = args[i + 3];
                            this.setView("ChooseConnection", "debug");
                        }

                        i += 3;
                    }                                        
                    else
                    {
                        string errorMessage = "Param `--auto-connect` requires syntax: --auto-connect <server> <username> <password>";
                        throw new ApplicationParametersException(errorMessage);
                    }

                }
                else if (args[i].ToLower() == "--open-window")
                {
                    if (OpenViewsOnStart == null)
                    {
                        OpenViewsOnStart = new List<string>();
                    }

                    if (i + 1 < args.Length)
                    {
                        OpenViewsOnStart.Add(args[i + 1]);
                        i++;
                    }
                    else
                    {
                        string errorMessage = "Param `--open-window` requires syntax: --open-window <window name>";
                        throw new ApplicationParametersException(errorMessage);
                    }
                }
                else if (args[i].ToLower() == "--play-network-game")
                {
                    int leagueId, roundId;

                    if (int.TryParse(args[i + 1], out leagueId) && int.TryParse(args[i + 2], out roundId))
                    {
                        NetworkGame = new Dictionary<string, string>();
                        NetworkGame.Add("LeagueID", args[i + 1]);
                        NetworkGame.Add("RoundID", args[i + 2]);
                        i += 2;
                    }
                    else
                    {
                        string errorMessage = "Param `--play-network-game` requires syntax: --play-network-game <league_id> <round_id>";
                        throw new ApplicationParametersException(errorMessage);
                    }                
                }
                else if (args[i].ToLower() == "--log-exclude-tags" || args[i].ToLower() == "--log-permit-tags")
                {
                    if (DebuggerIX_Tags == null)
                    {
                        DebuggerIX_Tags = new Dictionary<DebuggerTag, bool>();
                    }

                    DebuggerIX_Tags.Clear();
                    bool flag = (args[i].ToLower() == "--log-exclude-tags") ? true : false;

                    // Set defaults

                    // No excluded tags by default
                    foreach (DebuggerTag tag in Enum.GetValues(typeof(DebuggerTag)))
                    {
                        DebuggerIX_Tags.Add(tag, !flag);
                    }

                    i++;

                    if (i >= args.Length) // there's no next parameter
                    {
                        string errorMessage = "Param `" + args[i].ToLower() + "` requires syntax: --" + args[i].ToLower() + " <tag_name>+ [;]";
                        throw new ApplicationParametersException(errorMessage);
                    }

                    
                    while (i < args.Length && args[i] != ";")
                    {                        
                        try
                        {
                            DebuggerTag tag = (DebuggerTag)Enum.Parse(typeof(DebuggerTag), args[i]);
                            if (DebuggerIX_Tags.ContainsKey(tag))
                            {
                                DebuggerIX_Tags[tag] = flag;
                            }
                            else
                            {
                                DebuggerIX_Tags.Add(tag, flag);
                            }
                        }
                        catch (ArgumentException e)
                        {
                            DebuggerIX.WriteLine(DebuggerTag.AppParamsParsing, "[" + args[i].ToLower() + "]", 
                                "Param #" + (i+1) + " `"+args[i]+"` cannot be parsed. Exception: " + e.Message);
                        }

                        i++;
                    }

                    i++;
                }
            }
        }
    }
}
