using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Sokoban.Configuration;
using System.Configuration;

namespace Sokoban.Model.Profile
{
    public class GameServers
    {
        public ObservableCollection<string> GetGameServers()
        {
            ServerConfigurationSection col = (ServerConfigurationSection)ConfigurationManager.GetSection("Sokoban.GameServers");
            ObservableCollection<string> result = new ObservableCollection<string>();

            foreach (ServerElement item in col.Servers)
            {
                result.Add((string)item.Name);
            }

            return result;
        }
    }
}
