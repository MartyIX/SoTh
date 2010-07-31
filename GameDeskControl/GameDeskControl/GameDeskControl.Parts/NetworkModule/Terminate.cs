using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Sokoban.Lib;
using Sokoban.Networking;
using System.Diagnostics;

namespace Sokoban.View.GameDocsComponents
{
    public partial class NetworkModule
    {
        private bool isTerminated = false;

        public void Terminate()
        {
            if (isTerminated == false)
            {
                Terminate(false, NetworkMessageType.None, null);
            }
        }

        public void Terminate(bool disconnectionAlreadyBegun, NetworkMessageType nmt, object data)
        {            
            if (networkConnection != null && networkConnection.IsInitialized && isTerminated == false)
            {
                isTerminated = true;

                try
                {
                    bool isWorking = true;
                    bool received = false;
                    int state = disconnectionAlreadyBegun ? 2 : 0;
                    MessageBoxShow("Initiating disconnection of opponent..");
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    int attempts = 0;

                    while (isWorking && attempts < 4 && stopWatch.ElapsedMilliseconds < 10000)
                    {
                        if (state == 1)
                        {
                            networkConnection.ReceiveAsync();
                            received = networkConnection.ReceivedMessageHandle.WaitOne(3000);

                            if (received)
                            {
                                nmt = networkConnection.GetReceivedMessageType();
                                data = networkConnection.GetReceivedMessageFromQueue();
                            }
                            /*else
                            {
                                this.AppendMessage("No message received.");
                            }*/

                            state = (received) ? 2 : 1;
                        }

                        if (state == 0)
                        {
                            networkConnection.SendAsync(NetworkMessageType.DisconnectRequest, new DisconnectRequest(DateTime.Now));
                            AppendMessage("Sending disconnection request..");
                            state = 1;
                        }
                        else if (state == 2)
                        {
                            if (nmt == NetworkMessageType.DisconnectRequest)
                            {
                                this.AppendMessage("Answering disconnect request..");
                                networkConnection.SendAsync(NetworkMessageType.DisconnectRequestConfirmation,
                                    new DisconnectRequestConfirmation(DateTime.Now));
                                networkConnection.AllSentHandle.WaitOne(3000);
                                isWorking = false;
                            }
                            else if (nmt == NetworkMessageType.DisconnectRequestConfirmation)
                            {
                                this.AppendMessage("Disconnect request confirmed");
                                isWorking = false;
                            }
                            else if (nmt != NetworkMessageType.None)
                            {
                                attempts--; // add for SimulationEvent message and so on
                            }
                        }

                        state = 1;
                        received = false;
                        attempts++;
                    }

                    this.AppendMessage("Closing connection..");
                    networkConnection.CloseConnection();
                }
                catch (SocketException e)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, "Terminate", "Disconnection failed. SocketException: " + e.Message);
                    this.AppendMessage("Connection closed with error message: " + e.Message);
                }

                this.AppendMessage("Connection closed.");
            }
        }

    }
}
