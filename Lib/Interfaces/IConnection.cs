using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Interfaces;
using System.Threading;

namespace Sokoban.Networking
{
    public interface IConnection
    {       
        // NetworkEvents in buffer; local actions
        void AddEventToBuffer(Int64 time, int gameObjectID, EventType what, int posX, int posY);
        int InBufferCount { get; }

        // Network actions
        void InitializeConnection();
        void ReceiveAsync();
        void ReceiveAsync(bool forceReceiving);
        void SendAsync(NetworkMessageType nmt);
        void SendAsync(NetworkMessageType nmt, object data);
        void CloseConnection();
        bool IsInitialized { get; }

        bool IsPortAvailable(int port);
        EventWaitHandle ReceivedMessageHandle { get; }
        EventWaitHandle AllSentHandle { get; }
        NetworkMessageType GetReceivedMessageType();
        object GetReceivedMessageFromQueue();
    }
}
