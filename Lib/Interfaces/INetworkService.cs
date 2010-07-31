using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Interfaces
{
    public interface INetworkService
    {
        void SendRestartMessage();
        void SendNetworkEvent(int ID, Int64 when, EventType what, int posX, int posY);
        void ProcessNetworkTraffic(Int64 time);
        event GameChangeDelegate GameChanged;
        void Terminate();
    }
}
