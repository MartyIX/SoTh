using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Networking;

namespace Sokoban.Interfaces
{
    public interface IGameMatch
    {
        void SetNetworkConnection(IConnection connection);
    }
}
