using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Sokoban.Interfaces;
using Sokoban.Lib;
using Sokoban.Networking;
using System.Threading;
using System.Net.Sockets;
using System.Windows;
using Sokoban.Model;
using Sokoban.Lib.Exceptions;

namespace Sokoban.View.GameDocsComponents
{
    public partial class NetworkModule : INetworkService
    {        
        private int networkBufferSize = 0;
        private int networkReceivedData = 1;
        private bool isNetworkActive = true;
        
        private Stopwatch sendTimeOut = new Stopwatch();
        private IConnection networkConnection = null;
        private Game gameOpponent;
        private IUserInquirer userInquirer = null;

        private bool wasGameChangeSent = false;
        private GameChange gameChange = GameChange.None;
        private GameChange gameChangeOpponent = GameChange.None;
        //private bool gameWasWon = false;
        // private bool gameWasWonOpponent = false;
        private Stopwatch elapsedFromStartOfGame = new Stopwatch();
        private TimeSpan elapsedFromStartOfGameOpponent = TimeSpan.Zero;
        private Int64 time = 1;
        private Queue<NetworkEvent> queue;

        public event GameChangeDelegate GameChanged;
        public event VoidObjectDelegate DisconnectRequest;

        public NetworkModule(IUserInquirer userInquirer)
        {
            this.userInquirer = userInquirer;
            initialize();
        }

        public NetworkModule(IConnection networkConnection, Game gameOpponent, IUserInquirer userInquirer)
        {
            initialize();

            this.networkConnection = networkConnection;
            this.gameOpponent = gameOpponent;
            this.userInquirer = userInquirer;

            if (gameOpponent == null) throw new InvalidStateException("NetworkModule: Opponent is not set!");
        }

        private void initialize()
        {
            elapsedFromStartOfGame.Reset();
            elapsedFromStartOfGame.Start();

            sendTimeOut.Reset();
            sendTimeOut.Start();

             queue = new Queue<NetworkEvent>();
        }
        
        public void SetNetworkConnection(IConnection networkConnection)
        {
            this.networkConnection = networkConnection;

            this.sendDataToOpponent(1); // some events may be buffered already
        }

        public void SetGameOpponent(Game gameOpponent)
        {
            this.gameOpponent = gameOpponent;
        }

        public bool IsNetworkConnectionSetUp
        {
            get
            {
                return networkConnection != null;
            }
        }

        public void SendRestartMessage()
        {
            SendNetworkEvent(-1, -1, EventType.restartGame, -1, -1);
            sendDataToOpponent(this.time);
        }
        
        public void SendNetworkEvent(int ID, long when, EventType what, int posX, int posY)
        {
            networkBufferSize++;

            if (networkConnection == null)
            {
                // buffer events until connection is available
                queue.Enqueue(new NetworkEvent(networkBufferSize, when, ID, what, posX, posY));
            }
            else
            {
                // put buffered events to the queue for sending
                if (queue.Count > 0)
                {
                    while (queue.Count > 0)
                    {
                        NetworkEvent ne = queue.Dequeue();
                        networkConnection.AddEventToBuffer(ne.SimulationTime, ne.GameObjectID, ne.EventType, ne.PosX, ne.PosY);
                    }
                }

                // send 
                networkConnection.AddEventToBuffer(when, ID, what, posX, posY);

                if (what == EventType.gameWon)
                {
                    gameChange = GameChange.Won;
                }
            }
        }

        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }

        private void AppendMessage(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.AppendMessage(message);
            }
        }

    }
}
