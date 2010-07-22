using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Networking;

namespace Sokoban.Interfaces
{
    public interface IConnectionRelayer
    {
        void Connect(IConnection connection, IGameMatch gameMatch, Authentication autentization, int leaguesID, int roundsID);        
    }
}
