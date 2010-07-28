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
        private string role = "Client";        
        private string errorMessage;
        public string ErrorMessage { get {return errorMessage;} }

        public NetworkClient(string ipAddress, int port)
            : base("Client")
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

            errorMessage = "";

            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Get the remote IP address
                IPAddress ip = IPAddress.Parse(this.ipAddress);
                // Create the end point 
                IPEndPoint ipEnd = new IPEndPoint(ip, this.port);
                // Connect to the remote host
                clientSocket.Connect(ipEnd);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);


                if (clientSocket.Connected)
                {
                    isInitialized = true;
                }
            }
            catch (SocketException e)
            {
                isInitialized = false;
                DebuggerIX.WriteLine(DebuggerTag.Net, "[ClientInitConnection]", "Connection failed, is the server running? " + e.Message);
                errorMessage = e.Message;
            }
            catch (NullReferenceException e)
            {
                isInitialized = false;
                DebuggerIX.WriteLine(DebuggerTag.Net, "[ClientInitConnection]", "Connection failed, is the server running? " + e.Message);
                errorMessage = e.Message;
            }
        }

        public override void CloseConnection()
        {
            base.CloseConnection();
        }

        #endregion
    }
}
