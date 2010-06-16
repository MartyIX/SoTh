using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Sokoban.Model
{
    public interface IProfileRepository : IBaseRepository
    {
        bool TryLogin(string server, string username, string password);
        void SkipLogin();
        string LoginMessage { get; }
        event PropertyChangedEventHandler PropertyChanged;
        List<string> GetServers();
    }
}
