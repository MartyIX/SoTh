﻿using System;
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
        private IAsyncResult currentAsyncResult = null;

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
            /*
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
            }*/

            currentAsyncResult = client.BeginConnect(serverEndPoint, new AsyncCallback(acceptConnect), null);

            // Wait until the operation completes.
            waitHandle(currentAsyncResult, 5000);


        }

        public override void CloseConnection()
        {
            base.CloseConnection();
        }
        
        #endregion

        private void acceptConnect(IAsyncResult asyn)
        {
            if (currentAsyncResult != asyn) return; // http://rajputyh.blogspot.com/2010/04/solve-exception-message-iasyncresult.html
            
            try
            {
                client.EndConnect(asyn);
                isInitialized = true;
                isClosed = false;
            }
            catch (ObjectDisposedException)
            {
                DebuggerIX.WriteLine("[Net]", role, "Socket has been closed.");
                isInitialized = false;
            }
            catch (SocketException se)
            {
                DebuggerIX.WriteLine("[Net]", role, "Exception: " + se.Message);
                isInitialized = false;
            }

            if (isInitialized)
            {
                DebuggerIX.WriteLine("[Net]", role, "OK; Handling  client  at  " +
                    client.RemoteEndPoint.AddressFamily);
            }


            //initializeWaitHandle.Set();
        }
    }
}
