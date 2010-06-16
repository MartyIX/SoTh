using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Sokoban
{
    public interface ITcpCommunication
    {
        #region Operations (4)

        bool isConnected();

        int ReadData(ref byte[] receivedMessage);
        void SendData(byte[] message);
        void SendData(byte[] message, int size);
        void Disconnect();

        #endregion Operations
    }
}