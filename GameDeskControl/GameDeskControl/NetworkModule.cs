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
    public class NetworkModule : INetworkService
    {        
        private int networkBufferSize = 0;
        private int networkReceivedData = 1;
        private bool isNetworkActive = true;
        private Stopwatch sendTimeOut = new Stopwatch();
        private IConnection networkConnection = null;
        private Game gameOpponent;
        private IUserInquirer userInquirer = null;

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
            sendTimeOut.Reset();
            sendTimeOut.Start();
        }

        public void SetNetworkConnection(IConnection networkConnection)
        {
            this.networkConnection = networkConnection;
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

        public void SendNetworkEvent(int ID, long when, EventType what, int posX, int posY)
        {
            if (networkConnection == null) throw new InvalidStateException("NetworkModule: Network connection is not set!");

            networkBufferSize++;
            networkConnection.AddEventToBuffer(when, ID, what, posX, posY);
        }

        public void ProcessNetworkTraffic(long time)
        {
            if (networkConnection == null) throw new InvalidStateException("NetworkModule: Network connection is not set!");

            try
            {
                if (isNetworkActive == true)
                {
                    if (networkBufferSize > 3 || sendTimeOut.ElapsedMilliseconds > 50)
                    {                        
                        networkConnection.SendAsync(NetworkMessageType.SimulationTime, new SimulationTimeMessage(time, DateTime.Now));
                        
                        networkConnection.SendAsync(NetworkMessageType.ListOfEvents);
                        networkBufferSize = 0;

                        sendTimeOut.Reset();
                        sendTimeOut.Start();
                    }

                    if (networkReceivedData == 1)
                    {
                        networkConnection.ReceiveAsync();
                        Interlocked.Decrement(ref networkReceivedData);
                    }
                    else
                    {
                        if (networkConnection.ReceivedMessageHandle.WaitOne(0) == true)
                        {
                            if (gameOpponent.GameStatus == GameStatus.Unstarted)
                            {
                                gameOpponent.StartGame();
                            }

                            NetworkMessageType messageType = networkConnection.GetReceivedMessageType();

                            object obj = null;

                            if (messageType != NetworkMessageType.None)
                            {
                                obj = networkConnection.GetReceivedMessageFromQueue();
                            }

                            if (messageType == NetworkMessageType.ListOfEvents)
                            {                                
                                Queue<NetworkEvent> l = obj as Queue<NetworkEvent>;

                                if (l != null)
                                {
                                    DebuggerIX.WriteLine(DebuggerTag.Net, "ProcessNetworkTraffic", "List was received");

                                    while (l.Count > 0)
                                    {
                                        NetworkEvent ev = l.Dequeue();
                                        DebuggerIX.WriteLine(DebuggerTag.Net, "ProcessNetworkTraffic", ev.ToString());
                                        gameOpponent.GameRepository.MakePlan(ev.GameObjectID, ev.SimulationTime, ev.EventType);
                                    }
                                }
                                else
                                {
                                    DebuggerIX.WriteLine(DebuggerTag.Net, "runTest01", "Error: list was not received");
                                }
                            }
                            else if (messageType == NetworkMessageType.SimulationTime) 
                            {
                                SimulationTimeMessage stm = obj as SimulationTimeMessage;

                                gameOpponent.GameRepository.Time = (stm.SimulationTime - 10 < 1) ? 1 : stm.SimulationTime - 10;
                            }
                            else
                            {
                                networkConnection.GetReceivedMessageFromQueue();
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                isNetworkActive = false;
                MessageBoxShow("Network connection error: " + e.Message);
            }
        }

        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }
    }
}
