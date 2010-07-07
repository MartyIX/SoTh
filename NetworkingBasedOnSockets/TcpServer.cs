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

namespace Sokoban.Networking
{
    public class NetworkServer : TcpConnection, IConnection
    {
        private Socket socketMe = null;
        private string ipAddress;
        private int port;
        private EventWaitHandle initializeWaitHandle;
        private string role = "Server";

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
            
            if (isInitialized == false)
            {
                isClosed = true;

                if (socketMe == null)
                {
                    try
                    {                        
                        //  Create  a  socket  to  accept  client  connections
                        socketMe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socketMe.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        socketMe.Bind(new IPEndPoint(IPAddress.Any, this.port));
                    }
                    catch (SocketException se)
                    {
                        DebuggerIX.WriteLine("[Net]", role, se.ErrorCode + ":  " + se.Message);
                        return;
                    }

                    try
                    {
                        socketMe.Listen(this.Backlog);
                    }
                    catch (SocketException se)
                    {
                        DebuggerIX.WriteLine("[Net]", role, se.ErrorCode + ":  " + se.Message);
                        return;
                    }                

                }


                client = null;

                //try
                //{
                //client = socketMe.Accept();  //  Get  client  connection                
                
                initializeWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                
                IAsyncResult result = socketMe.BeginAccept(new AsyncCallback(acceptFinished), null);

                // Wait until the operation completes.

                initializeWaitHandle.WaitOne();

                DebuggerIX.WriteLine("[Net]", role, "Initialization succesfully left");

                // now we have variable @client set!
                //}
                //catch (Exception e)
                //{
                //    DebuggerIX.WriteLine("[Net]", "[InitializeConnection]", "Exception: " + e.Message);
                //    this.CloseConnection();
                //}
            }
        }

        // This is the call back function, which will be invoked when a client is connected
        private void acceptFinished(IAsyncResult asyn)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                client = socketMe.EndAccept(asyn);
                isInitialized = true;
                isClosed = false;
            }
            catch (ObjectDisposedException)
            {
                DebuggerIX.WriteLine("[Net]", role, "Socket has been closed.");
            }
            catch (SocketException se)
            {
                DebuggerIX.WriteLine("[Net]", role, "Exception: " + se.Message);
            }

            if (isInitialized)
            {
                DebuggerIX.WriteLine("[Net]", role, "OK; Handling  client  at  " + 
                    client.RemoteEndPoint.AddressFamily);
            }


            initializeWaitHandle.Set();
        }

        public override void CloseConnection()
        {
            base.CloseConnection();
            //base.CloseConnection(socketMe);    
        }

    }
}