using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Sokoban.Lib;
using System.IO;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Networking
{
    public class NetworkClient : TcpConnection, IConnection
    {
        private string ipAddress;
        private int port;

        public NetworkClient(string ipAddress, int port) : base("Client")
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }


        #region IConnection Members

        public void InitializeConnection()
        {
            isInitialized = false;
            isClosed = true;

            receivingInit();
            sendingInit();

            /*
            if (!IsPortAvailable(this.port))
            {
                throw new InvalidStateException("Port " + this.port + " is not available. Try different.");
            }*/

            //  Create a TCP socket  instance; AddressFamily.InterNetwork - Address for IP version 4
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPAddress ipAddress = new IPAddress(IPAddress.Parse(this.ipAddress).Address);

            //  Creates  server  IPEndPoint  instance. We  assume Resolve returns at least one address
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, this.port);

            //  Connect the socket to server on specified  port
            try
            {
                client.Connect(serverEndPoint);
                DebuggerIX.WriteLine("[Net]", "[ClientInitConnection]", "Connected to: " + this.ipAddress + ":" + this.port.ToString());
                isInitialized = true;
                isClosed = false;
            }
            catch (Exception e)
            {
                DebuggerIX.WriteLine("[Net]", "[ClientInitConnection]", "Failed: " + e.Message);                
            }
        }

        public override void CloseConnection()
        {
            base.CloseConnection();
        }
        
        #endregion
    }
}
