using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Networking;
using System.Net.Sockets;
using System.Threading;

namespace Sokoban.View.GameDocsComponents
{
    public partial class NetworkModule
    {
        private void checkWhoWon()
        {
            // Somebody won
            if (gameChange != GameChange.None && gameChangeOpponent != GameChange.None)
            {
                if (gameChange == GameChange.Won)
                {
                    //MessageBoxShow("You have won!");
                    if (GameChanged != null)
                    {
                        GameChanged(GameDisplayType.FirstPlayer, gameChange);
                    }
                }
                else if (gameChangeOpponent == GameChange.Won)
                {
                    //MessageBoxShow("Your opponent have won!");
                    if (GameChanged != null)
                    {
                        GameChanged(GameDisplayType.FirstPlayer, gameChange);
                    }
                }
            }
        }

        private void sendDataToOpponent(long time)
        {
            //networkConnection.SendAsync(NetworkMessageType.SimulationTime, new SimulationTimeMessage(time, elapsedFromStartOfGame.Elapsed));
            networkConnection.SendAsync(NetworkMessageType.ListOfEvents, time);
            networkBufferSize = 0;

            if (gameChange == GameChange.Won)
            {
                elapsedFromStartOfGame.Stop();
                wasGameChangeSent = true;
                networkConnection.SendAsync(NetworkMessageType.GameChange,
                    new GameChangeMessage(gameChange, elapsedFromStartOfGame.Elapsed));
            }
        }


        /// <summary>
        /// Called from Round.cs, method OnRender
        /// </summary>
        /// <param name="time"></param>
        public void ProcessNetworkTraffic(long time)
        {
            this.time = time;

            if (networkConnection != null && networkConnection.IsInitialized == true)
            {
                try
                {
                    if (isNetworkActive == true)
                    {
                        if (networkBufferSize > 3 || sendTimeOut.ElapsedMilliseconds > 50 || gameChange == GameChange.Won)
                        {
                            sendDataToOpponent(time);

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
                                    ListOfEventsMessage message = obj as ListOfEventsMessage;

                                    if (message != null)
                                    {
                                        this.receivedListOfEvents(message);                                        
                                    }
                                    else
                                    {
                                        DebuggerIX.WriteLine(DebuggerTag.Net, "runTest01", "Error: list was not received");
                                    }
                                }
                                else if (messageType == NetworkMessageType.DisconnectRequest)
                                {
                                    this.receivedDisconnectRequest(obj);
                                }
                                else if (messageType == NetworkMessageType.SimulationTime)
                                {
                                    SimulationTimeMessage stm = obj as SimulationTimeMessage;
                                    if (stm != null)
                                    {
                                        gameOpponent.GameRepository.Time = stm.SimulationTime - 10;
                                    }
                                }
                                else if (messageType == NetworkMessageType.GameChange)
                                {                                                                        
                                    GameChangeMessage gw = obj as GameChangeMessage;
                                    this.receivedGameChange(gw);                                    
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
        }

        private void receivedDisconnectRequest(object data)
        {
            if (DisconnectRequest != null)
            {
                DisconnectRequest(data);                
            }
        }

        private void receivedGameChange(GameChangeMessage gw)
        {
            gameChangeOpponent = gw.GameChange;
            elapsedFromStartOfGameOpponent = gw.ElapsedFromStartOfGame;

            if (gameChangeOpponent == GameChange.Won && gameChange == GameChange.None)
            {
                gameChange = GameChange.Lost;
            }
            else if (gameChangeOpponent == GameChange.Won && gameChange == GameChange.Won)
            {
                int comparation = TimeSpan.Compare(elapsedFromStartOfGame.Elapsed, elapsedFromStartOfGameOpponent);

                if (comparation == -1) // elapsedFromStartOfGame is shorter
                {
                    gameChangeOpponent = GameChange.Lost;
                }
                else if (comparation == 0)
                {
                    gameChangeOpponent = GameChange.Tie;
                    gameChange = GameChange.Tie;
                }
                else
                {
                    gameChange = GameChange.Lost;
                }
            }

            if (wasGameChangeSent == false)
            {
                networkConnection.SendAsync(NetworkMessageType.GameChange,
                    new GameChangeMessage(gameChange, elapsedFromStartOfGame.Elapsed));
            }

            checkWhoWon();
        }

        private void receivedListOfEvents(ListOfEventsMessage message)
        {
            Queue<NetworkEvent> l = message.Events;
            gameOpponent.GameRepository.Time = message.SimulationTime - 10;

            DebuggerIX.WriteLine(DebuggerTag.Net, "ProcessNetworkTraffic", "List was received");

            while (l.Count > 0)
            {
                NetworkEvent ev = l.Dequeue();
                DebuggerIX.WriteLine(DebuggerTag.Net, "ProcessNetworkTraffic", ev.ToString());
                gameOpponent.GameRepository.MakePlan(ev.GameObjectID, ev.SimulationTime, ev.EventType);
            }
        }

    }
}
