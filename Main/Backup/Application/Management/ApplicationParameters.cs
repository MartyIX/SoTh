using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Application
{
    public class ApplicationParameters
    {
        public Dictionary<string, string> views = null;
        public Dictionary<string, string> credentials = null;
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

                if (args[i].ToLower() == "--auto-connect")
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
                    }
                    else
                    {
                        string errorMessage = "Param `--open-window` requires syntax: --open-window <window name>";
                        throw new ApplicationParametersException(errorMessage);
                    }
                }
            }
        }
    }
}
