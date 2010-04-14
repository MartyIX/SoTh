using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Sokoban
{
    public class TcpHost : ITcpCommunication
    {
        #region Fields (7)

        private ASCIIEncoding enc;
        private TcpListener listener;
        private int port;
        private Socket s;
        private bool state;

        #endregion Fields

        #region Constructors (1)

        public TcpHost()
        {
            state = false;
            enc = new ASCIIEncoding();
        }

        #endregion Constructors

        #region Methods (6)

        // Public Methods (6) 

        /// <summary>
        /// Disconnects if connection is established
        /// </summary>
        public void Disconnect()
        {
            if (isConnected())
            {
                GameDeskView.Debug("Disconnecting: stopping listening", "Player2Net");

                state = false;
                s.Close();
                listener.Stop();
            }
            else
            {
                GameDeskView.Debug("Disconnecting: connection is not established", "Player2Net");
            }
        }

        public bool isConnected()
        {
            return (s != null) ? s.Connected : false;
        }

        public int ReadData(ref byte[] receivedMessage)
        {
            if (state == true)
            {
                if (s.Available > 0) // if there's any data
                {
                    return s.Receive(receivedMessage);
                }
            }

            return 0;
        }

        public void SendData(byte[] message)
        {
            if (state == true)
            {
                s.Send(message);
            }
        }

        public void SendData(byte[] message, int size)
        {
            if (state == true)
            {
                s.Send(message, size, SocketFlags.None);
            }
        }

        public void Start(string p_ipAddress, int listenPort)
        {
            IPAddress ipAddress = IPAddress.Parse(p_ipAddress);
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, listenPort);

            listener = new TcpListener(ipLocalEndPoint);
            listener.ExclusiveAddressUse = false; 

            port = listenPort;
            listener.Start();
            GameDeskView.Debug("Server is running");
            GameDeskView.Debug("Listening on port " + listenPort);
            GameDeskView.Debug("Waiting for connections...");

            Dialogs.EstablishingConnection dialog = new Dialogs.EstablishingConnection();
            dialog.Show();
            dialog.Focus();

            while (true)
            {
                if (dialog.DialogResult == DialogResult.Cancel)
                {
                    GameDeskView.Debug("Listenning aborted.");
                    listener.Stop();
                    break;
                }
                else if (!listener.Pending())
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(50);
                }
                else
                {
                    s = listener.AcceptSocket();
                    GameDeskView.Debug("Connection accepted from " + s.RemoteEndPoint, "Player1Net");
                    dialog.Close();
                    break;
                }
            }

            state = true;
        }

        #endregion Methods
    }
}