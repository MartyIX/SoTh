//#define simulation_of_instability_of_network

#region USINGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using System.Xml;
#endregion USINGS

namespace Sokoban
{
    public class SecondPlayer : Player, IDisposable
    {
		#region Fields (9) 

        /// <summary>
        /// Is second player client or server in TCP communication
        /// </summary>
        public PlayerType playerType;

        /// <summary>
        /// Thread which sends and receives data from opponent
        /// </summary>
        Thread dataExchangeThread;

        /// <summary>
        /// Buffer with event which are about to be sent
        /// </summary>
        Queue<Event> eventBuffer;

        /// <summary>
        /// sending buffer
        /// </summary>
        byte[] messageToSend;

        /// <summary>
        /// Class for encoding and decoding sent/received messages
        /// </summary>
        NetworkMessageProtocol nmp;

        /// <summary>        
        /// If user is peer (playerType contains the information) it contains valid pointer, otherwise null
        /// </summary>
        TCPClient peer = null;

        /// <summary>        
        /// If user is server (playerType contains the information) it contains valid pointer, otherwise null
        /// </summary>
        TcpHost server = null;

        /// <summary>
        /// receiving buffer
        /// </summary>
        byte[] receivedMessage;

        /// <summary>
        /// Time when round is finished
        /// </summary>
        Int64 victoryTimeTicks = 0;
        #if simulation_of_instability_of_network
            Random rnd = new Random(10);
        #endif

        

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Class Player creates gamedesk
        /// </summary>
        public SecondPlayer(GameDeskView form, int posX, int posY) : base(form, posX, posY)
        {
            this.form = form;
            eventBuffer = new Queue<Event>();
            playerType = PlayerType.NotDecided;            
            messageToSend = new byte[1200];
            receivedMessage = new byte[1200];
            nmp = new NetworkMessageProtocol();
            nmp.NetworkTimeChanged += new d_NetworkTimeChanged(SynchronizeWithNetworkTime);
            nmp.EventWasRead += new d_SimulationEventHandler(OnEventWasRead);
            nmp.PlayerFinishedRound += new d_PlayerFinishedRound(ReceivedVictoryMessage);
        }

		#endregion Constructors 

		#region Delegates and Events (1) 

		// Events (1) 

        public event d_PlayerConnectionMessages ConnectionMessages;

		#endregion Delegates and Events 

		#region Methods (21) 

		// Public Methods (21) 

        public void AddEventToBuffer(Event ev)
        {
            eventBuffer.Enqueue(ev);
        }

        /// <summary>
        /// Not used at the moment - will be used if transfer of data over TCP will be too slow and the first player will 
        /// be affected by this
        /// </summary>
        public void DataExchangeLoop()
        {
            while (true)
            {
                try
                {
                    ReadDataFromOpponent();
                    SendEventsToOpponent();

                    if (GetConnection().isConnected() == false && ConnectionMessages != null && form.IsDisposed == false)
                    {
                        form.Invoke(ConnectionMessages, (int)1); // 1 = connection was lost
                    }                 
                }                
                catch (Exception e)
                {
                    GameDeskView.Debug("An error in network communication occured: " + e.Message, "general");
                }

                if (!GetConnection().isConnected())
                {
                    if (!ReestablishConnection(5))
                    {
                        break;
                    }
                }

                #if simulation_of_instability_of_network
                    Thread.Sleep(rnd.Next(5, 500));
                #else
                    Thread.Sleep(5);
                #endif
            }
        }

        /// <summary>
        /// More general version of function "Reconnect()"
        /// </summary>
        /// <param name="maxAttempts">Maximal number of attempts to reconnect</param>
        /// <returns></returns>
        public bool ReestablishConnection(int maxAttempts)
        {
            bool ret = true;

            if (ConnectionMessages != null && form.IsDisposed == false)
            {
                for (int i = 0; i < maxAttempts; i++)
                {
                    Reconnect();

                    // connection was succesful
                    if (GetConnection().isConnected() == true)
                    {
                        break;
                    }
                }

                // Connection is definitely lost, no more reconnect attempts
                if (GetConnection().isConnected() == false)
                {
                    ret = false;
                }
            }

            if (ret == true)
            {
                form.Invoke(ConnectionMessages, (int)2); // 2 = Connection was established
            }

            return ret;
        }

        /// <summary>
        /// Tries to establish interrupted connection
        /// </summary>
        public void Reconnect()
        {
            InitializeConnection(playerType);
        }

        public void Dispose()
        {
            if (dataExchangeThread != null)
            {
                try
                {
                    ConnectionMessages = null;
                    dataExchangeThread.Abort();
                    dataExchangeThread.Join(200);
                }
                catch (Exception e)
                {
                    GameDeskView.Debug("DataExchangeThread was not aborted. Message: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Send all events in buffer to opponent
        /// </summary>
        public void FlushEventBuffer()
        {
            DebugP("Event buffer will be flushed.");
            int i = 0;

            while (eventBuffer.Count > 0)
            {
                i++;
                SendEventsToOpponent();

                if (i > 100)
                {
                    throw new Exception("Flushing of messages went into an infinite loop.");
                }
            }
            DebugP("Event buffer was flushed.");
        }

        public ITcpCommunication GetConnection()
        {
            ITcpCommunication who = server; // PlayerType.One
            if (playerType == PlayerType.Client) who = peer;

            return who;
        }

        /// <summary>
        /// We fill the buffer of events to be send
        /// </summary>
        /// <param name="ev"></param>
        public void OnAddedEvent(int eventID, int objectID, Int64 when, EventType what, int posX, int posY)
        {
            eventBuffer.Enqueue(new Event(eventID, when, gameDesk.gameObjects[objectID], what, posX, posY));
        }

        /// <summary>
        /// Event was read from the opponent (over network)
        /// </summary>
        /// <param name="ev"></param>
        public void OnEventWasRead(int eventID, int objectID, Int64 when, EventType what, int posX, int posY)
        {
            Event ev = new Event(eventID, when, this.gameDesk.gameObjects[objectID], what, posX, posY);

            if (this.gameDesk.gameObjects.Count <= objectID || objectID < 0)
            {
                GameDeskView.Debug("[ProcessReceivedData] Bad message!\n", "NetworkReadingPackages");
            }
            else
            {
                if (ev.who == this.gameDesk.pSokoban)
                {
                    GameDeskView.Debug(ev.ToString(), "CalAdd2thPlayer");
                }

                // last true says to ignore disabled adding events to the calendar
                model.calendar.AddEvent(ev.when, ev.who, ev.what, ev.posX, ev.posY, true);
            }
        }


        /// <summary>
        /// One application is client and the other is server.
        /// </summary>
        public void InitializeConnection(PlayerType playerType)
        {
            if (playerType == PlayerType.Server)
            {
                DebugP("Listening and waiting for the opponent.");
                this.playerType = PlayerType.Server;
                server = new TcpHost();
                server.Start(form.tIPAddress.Text, int.Parse(form.tPort.Text));
            }
            else
            {
                DebugP("Connecting to the opponent.");
                this.playerType = PlayerType.Client;
                peer = new TCPClient();
                peer.Connect(form.tIPAddress.Text, int.Parse(form.tPort.Text)); // "85.207.250.239"
            }

            if (dataExchangeThread == null)
            {
                dataExchangeThread = new Thread(new ThreadStart(DataExchangeLoop));
                dataExchangeThread.Start();
            }

            this.gameState = GameState.Running;
        }

        public void ReadDataFromOpponent()
        {
            ITcpCommunication who = server; // PlayerType.One
            if (playerType == PlayerType.Client) who = peer;

            int received = who.ReadData(ref receivedMessage);

            if (received > 0)
            {
                nmp.DecodeData(receivedMessage, received);
            }
        }

        public void ReceivedVictoryMessage(Int64 ticksOfOpponent)
        {
            d_StringDelegate del = new d_StringDelegate(form.SetNetworkStatus);

            if (ticksOfOpponent == 0)
            {
                form.Invoke(del, new object[] {"You've won the game!"});
            }
            else 
            {
                SendLostGameMessage();
                form.Invoke(del, new object[] { "You've lost the game!" });
            }
        }

        /// <summary>
        /// We need to take all events from my calendar to send it to the second app
        /// </summary>
        /// <param name="player"></param>
        public void RegisterForeignCalendar(Player player)
        {            
            player.model.calendar.AddedEvent += new d_SimulationEventHandler(OnAddedEvent);
            DebugP("Calendar of this player was registered.");
        }

        public void SendEventsToOpponent()
        {
            if (eventBuffer.Count > 0 && playerType != PlayerType.NotDecided)
            {
                int messageLen = 0;
                if (nmp.EncodeEvents(ref eventBuffer, ref messageToSend, out messageLen, form.player.model.time) > 0)
                {
                    SendNetworkMessage(ref messageToSend, messageLen);
                }
            }            
        }

        public void SendLostGameMessage()
        {
            SendVictoryMessage(0);
        }

        public void SendNetworkMessage(ref byte[] message, int messageLen)
        {
            ITcpCommunication who = GetConnection();

            if (who.isConnected() == true)
            {
                who.SendData(message, messageLen);
            }
        }

        public void SendVictoryMessage()
        {
            victoryTimeTicks = DateTime.Now.Ticks;

            SendVictoryMessage(victoryTimeTicks);
        }

        public void SendVictoryMessage(Int64 ticks)
        {
            byte[] messageToSend = new byte[1200];
            
            int messageLen = nmp.EncodeVictoryMessage(ref messageToSend, victoryTimeTicks);

            SendNetworkMessage(ref messageToSend, messageLen);
        }

        /// <summary>
        /// If a round is loaded then function stop this round - time is stopped and entire discrete simulation of game is stopped.
        /// </summary>
        public override void StopGame()
        {
            DebugP("Game was stopped.");
            if (this.gameState != GameState.NotLoaded)
            {
                //if (dataExchangeThread != null) dataExchangeThread.Abort();
                this.gameState = GameState.Stopped;
            }
        }

        public void SynchronizeWithNetworkTime(Int64 networkTime)
        {
            int delayTime = int.Parse(form.tDelay.Text);
            int remainder = 0;
            int result = Math.DivRem(delayTime, 10, out remainder); // one tenth

            if (networkTime < this.model.time)
            {
                GameDeskView.Debug("Synchronizing: Local time (" + this.model.time + ") is greater than the network one ("+networkTime.ToString()+") \n", "NetworkTimeSynchronization");
                this.model.time = networkTime - delayTime;
            }

            if (networkTime - this.model.time < delayTime - result || networkTime - this.model.time > delayTime + result)
            {
                this.model.time = networkTime - delayTime;    // synchronize time
                GameDeskView.Debug("Synchronizing: Local time (" + this.model.time + ") is greater than the network one (" + networkTime.ToString() + ") \n", "NetworkTimeSynchronization");
            }
        }

        public override string ToString()
        {
            return "PlayerTwo";
        }

		#endregion Methods 

    }
}