using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Sokoban
{
    public class TCPClient : ITcpCommunication
    {
		#region Fields (6) 

        private NetworkStream con_ns;
        private TcpClient con_server;
        private int port;
        private string server;

		#endregion Fields 

		#region Constructors (1) 

        public TCPClient()
        {
        }

		#endregion Constructors 

		#region Methods (7) 

		// Public Methods (6) 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server">e.g., server = "127.0.0.1"</param>
        /// <param name="port">e.g., port = 9050</param>
        public void Connect(string server, int port) 
        {
            this.port = port;
            this.server = server;            

            try
            {
                con_server = new TcpClient(this.server, this.port);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Unable to connect to server" + ex.Message);
                return;
            }

            con_server.NoDelay = false;
            con_server.ReceiveBufferSize = 1200;
            con_server.SendBufferSize = 1200;
            con_server.SendTimeout = 1000;
            con_server.ReceiveTimeout = 1000;

            // Should send all data in buffer before closing of connection
            con_server.LingerState = new LingerOption(true, 1); 

            con_ns = con_server.GetStream();
        }

        /// <summary>
        /// Disconnects if connection is established
        /// </summary>
        public void Disconnect()
        {
            if (isConnected())
            {
                GameDeskView.Debug("Disconnecting from server..", "Player2Net");

                con_ns.Close();
                con_server.Close();
            }
            else
            {
                GameDeskView.Debug("Disconnecting: connection is not established", "Player2Net");
            }
        }

        public bool isConnected()
        {
            return (con_server != null) ? con_server.Connected : false;
        }

        public int ReadData(ref byte[] receivedMessage)
        {
            if (con_ns != null)
            {
                if (con_ns.DataAvailable)
                {
                    return con_ns.Read(receivedMessage, 0, receivedMessage.Length);                    
                }
            }
            else
            {
                GameDeskView.Debug("Warning: con_ns is not inicialized.","player2");
            }

            return 0;
        }

        public void SendData(byte[] message)
        {
            if (con_ns != null)
            {
                con_ns.Write(message, 0, message.Length);
            }
        }

        public void SendData(byte[] message, int size)        
        {
            if (con_ns != null)
            {
                con_ns.Write(message, 0, size);
            }
        }
		// Private Methods (1) 

        private void Debug(string message)
        {
            GameDeskView.Debug("Connected to: " + server + "port: " + port.ToString() + ": " + message, "Player2Net");
        }

		#endregion Methods 
    }
}