using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Sokoban.Lib;
using System.IO;

namespace Sokoban.Networking
{
    public class NetworkClient : TcpConnection, IConnection
    {
        private string ipAddress;
        private int port;

        public NetworkClient(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }


        #region IConnection Members

        public void InitializeConnection()
        {
            isInitialized = false;
            //  Create a TCP socket  instance; AddressFamily.InterNetwork - Address for IP version 4
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = new IPAddress(IPAddress.Parse(this.ipAddress).Address);

            //  Creates  server  IPEndPoint  instance. We  assume Resolve returns at least one address
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, this.port);

            //  Connect the socket to server on specified  port
            try
            {
                client.Connect(serverEndPoint);
                DebuggerIX.WriteLine("[Net]", "[InitializeConnection]", "Connected to: " + this.ipAddress + ":" + this.port.ToString());
                isInitialized = true;
            }
            catch (Exception e)
            {
                DebuggerIX.WriteLine("[Net]", "[InitializeConnection]", "Failed: " + e.Message);                
            }
        }

        public void CloseConnection()
        {
            client.Close();
        }
        
        #endregion
    }
}
