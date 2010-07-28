using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game : INetworkService
    {
        public void SendNetworkEvent(int ID, long when, EventType what, int posX, int posY)
        {
            if (networkService != null)
            {
                networkService.SendNetworkEvent(ID, when, what, posX, posY);
            }
        }

        public void ProcessNetworkTraffic(long time)
        {
            if (networkService != null)
            {
                networkService.ProcessNetworkTraffic(time);
            }
        }

    }
}
