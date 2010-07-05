using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Sokoban.Lib;

namespace Sokoban.Networking
{
    public class NetworkServer : TcpConnection, IConnection
    {
        private Socket socketMe;
        private string ipAddress;
        private int port;

        public int Backlog
        {
            get;
            set;
        }

        public NetworkServer(int port)
        {
            this.port = port;
        }
        
        public NetworkServer(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public void InitializeConnection()
        {
            socketMe = null;
            isInitialized = false;

            try
            {
                //  Create  a  socket  to  accept  client  connections
                socketMe = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

                socketMe.Bind(new IPEndPoint(IPAddress.Any, this.port));
                socketMe.Listen(this.Backlog);
            }
            catch (SocketException se)
            {
                DebuggerIX.WriteLine(se.ErrorCode + ":  " + se.Message);
            }

            client = null;

            try
            {
                client = socketMe.Accept();  //  Get  client  connection
                DebuggerIX.WriteLine("[Net]", "[InitializeConnection]", "OK; Handling  client  at  " + client.RemoteEndPoint.AddressFamily);
                isInitialized = true;
            }
            catch (Exception e)
            {
                DebuggerIX.WriteLine("[Net]", "[InitializeConnection]", "Exception: " + e.Message);
                this.CloseConnection();
            }            
        }
    }
}
