using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Networking
{
    public interface IConnection
    {       
        // NetworkEvents in buffer; local actions
        void AddEventToBuffer(Int64 time, int gameObjectID, EventType what, int posX, int posY);
        NetworkEvent GetIncommingEvent();
        int InBufferCount { get; }

        // Network actions
        void InitializeConnection();
        NetworkMessageType Receive();
        void Send(NetworkMessageType nmt);
        void CloseConnection();
        bool IsInitialized { get; }
    }
}
