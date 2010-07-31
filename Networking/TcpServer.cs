using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Sokoban.Lib;
using System.Threading;
using System.Net.NetworkInformation;
using Sokoban.Lib.Exceptions;
using System.Windows;

namespace Sokoban.Networking
{
    public class NetworkServer : TcpConnection, IConnection
    {
        private Socket mainSocket = null;
        private string ipAddress = null;
        private int port;
        private string role = "Server";
        private IAsyncResult currentAsyncResult = null;
        private Socket[] workerSocket = new Socket[10];
        private ManualResetEvent initializationWaitHandle;

        public int Backlog
        {
            get;
            set;
        }

        public NetworkServer(int port) : base("Server")
        {
            this.port = port;
        }

        public NetworkServer(string ipAddress, int port) : base("Server")
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public void InitializeConnection()
        {
            receivingInit();
            sendingInit();
            isInitialized = false;

            try
            {
                IPAddress _ip = (this.ipAddress == null || this.ipAddress == "Automatic") 
                    ? IPAddress.Any 
                    : IPAddress.Parse(this.ipAddress);
                
                // Create the listening socket...
                mainSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

                mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                mainSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, port);
                // Bind to local IP Address...
                
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Binding");
                mainSocket.Bind(ipLocal);
                DebuggerIX.WriteLine(DebuggerTag.Net, role, Thread.CurrentThread.Name + "; Start listening..");
                mainSocket.Listen(4);
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "BeginAccept for clients");
                currentAsyncResult = mainSocket.BeginAccept(new AsyncCallback(onClientConnect), null);
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Waiting on a client");
                
                initializationWaitHandle = new ManualResetEvent(false);
                waitHandle(initializationWaitHandle, 60000); // one minute
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Waiting done; IsConnected:" + mainSocket.Connected);
            }
            catch (SocketException se)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, se.ErrorCode + ":  " + se.Message);                
            }
        }


        // This is the call back function, which will be invoked when a client is connected
        private void onClientConnect(IAsyncResult asyn)
        {
            if (currentAsyncResult != asyn)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "OnClientConnect: Called with wrong AsyncResult.");
                return;
            }

            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                clientSocket = mainSocket.EndAccept(asyn);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                // http://msdn.microsoft.com/en-us/library/system.net.sockets.lingeroption(VS.71).aspx
                LingerOption myOpts = new LingerOption(true, 2);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, myOpts);

                isInitialized = true;
            }
            catch (ObjectDisposedException)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Socket has been closed.");
                isInitialized = false;
            }
            catch (SocketException se)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "SocketException: " + se.Message);
                isInitialized = false;
            }
            catch (ArgumentException e)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "ArgumentException: " + e.Message);
                isInitialized = false;
            }

            if (clientSocket != null && clientSocket.Connected)
            {
                isInitialized = true;
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "OK; Handling  client  at  " +
                    clientSocket.RemoteEndPoint.AddressFamily);
            }
            else
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "OnClientConnect: Connection failure");
            }

            initializationWaitHandle.Set();
        }

        public override void CloseConnection()
        {
            if (mainSocket != null)
            {
                mainSocket.Close();
            }
            
            base.CloseConnection();
        }

    }
}