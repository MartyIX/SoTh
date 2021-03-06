﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Sokoban.Interfaces;
using Sokoban.Model;
using Sokoban.Networking;
using System.Threading;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;

namespace Sokoban.View.SetupNetwork
{
    /// <summary>
    /// Interaction logic for ConnectDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window, INotifyPropertyChanged
    {

        // 
        // API
        //

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; Notify("IpAddress"); }
        }

        public int Port
        {
            get { return port; }
            set { port = value; Notify("Port"); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; Notify("Status"); }
        }

        //
        // Private fields
        //

        private delegate void AddMessageDelegate(string message);
        private IErrorMessagesPresenter errorPresenter = null;
        private IConnectionRelayer connectionRelayer = null;
        private IProfileRepository profile = null;
        private string ipAddress;        
        private int leaguesID;
        private int roundsID;
        private Thread t1;
        private int port;
        private int endTheThread;
        private Authentication auth = null;
        private string status = "";
        private IGameMatch gameMatch;


        public ConnectionDialog(string ipAddress, int port, int leaguesID, int roundsID, 
            IProfileRepository profileRepository, IErrorMessagesPresenter errorPresenter, 
            IConnectionRelayer connectionRelayer, IGameMatch gameMatch)
        {
            InitializeComponent();
            this.DataContext = this;            
            this.leaguesID = leaguesID;
            this.roundsID = roundsID;
            this.profile = profileRepository;
            this.errorPresenter = errorPresenter;
            this.connectionRelayer = connectionRelayer;
            this.port = port;
            this.ipAddress = ipAddress;
            this.gameMatch = gameMatch;
            endTheThread = 0;

            auth = new Authentication(profileRepository.Username, profileRepository.IPAddress);
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        private void errorMessage(ErrorMessageSeverity ems, string message)
        {
            if (errorPresenter != null)
            {
                errorPresenter.ErrorMessage(ems, "InitConnection", message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Terminate();
        }

        public void Terminate()
        {
            cancel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AddMessage(string message)
        {
            Status = Status + message + "\n";
        }


        private void establishConnection(IConnection connection, Authentication autentization)
        {
            if (connectionRelayer == null)
            {
                errorMessage(ErrorMessageSeverity.Medium,
                    "Connection is ready but it cannot be sent to main program. Error is unrecoverable.");
                MessageBox.Show("Connection is ready but it cannot be sent to main program.");
            }
            else
            {
                connectionRelayer.Connect(connection, this.gameMatch, autentization, leaguesID, roundsID);
            }
        }

        private void start()
        {
            if (t1 == null || t1.ThreadState == ThreadState.Stopped)
            {
                //  Create  a  ThreadStart  instance  using  your  method  as  a  delegate:
                ParameterizedThreadStart methodDelegate = new ParameterizedThreadStart(runClient);
                //  Create  a  Thread  instance  using  your  delegate  method:
                t1 = new Thread(methodDelegate);
            }

            if (t1.ThreadState == ThreadState.Unstarted)
            {
                endTheThread = 0;

                ThreadClientParam param = new ThreadClientParam(auth);
                t1.Start(param);
            }
        }


        private void cancel()
        {
            if (t1 != null && (t1.ThreadState == ThreadState.Running || t1.ThreadState == ThreadState.WaitSleepJoin))
            {
                t1.Interrupt();
                if (endTheThread == 0)
                {
                    Interlocked.Increment(ref endTheThread);
                }

            }
        }

        private void runClient(object obj)
        {
            ThreadClientParam param = obj as ThreadClientParam;

            string role = "Client";
            NetworkClient n = new NetworkClient(this.ipAddress, this.port);
            ConnectionEstablished threadStoppedDel = new ConnectionEstablished(threadStopped);

            int i = 1;
            AddMessageDelegate addMessage = new AddMessageDelegate(AddMessage);
            Dispatcher.Invoke(addMessage, "Connecting to: " + this.ipAddress + ":" + this.port);

            bool startGameMessageReceived = false;
            bool wasGameStarted = false;
            int maxNumberOfConnectionAttempts = 3;

            try
            {
                while (i <= maxNumberOfConnectionAttempts && endTheThread != 1)
                {
                    startGameMessageReceived = false;
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "Initialization # " + i + " invoked");
                    Dispatcher.Invoke(addMessage, "Attempt to receive connection #" + i);

                    try
                    {
                        n.InitializeConnection();
                    }
                    catch (InvalidStateException e)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "InvalidState: " + e.Message);
                        Dispatcher.Invoke(addMessage, "Error:" + e.Message);
                        continue;
                    }
                    catch (TimeoutException e)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "TimeOut: " + e.Message);
                        Dispatcher.Invoke(addMessage, "Timeout for attempt #" + i);
                        continue;
                    }

                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "Initialization = " + n.IsInitialized);

                    if (n.IsInitialized == false)
                    {
                        Dispatcher.Invoke(addMessage, n.ErrorMessage);
                    }
                    else 
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "Sending 'Autentization'");
                        n.SendAsync(NetworkMessageType.Authentication, param.Authentication);

                        try
                        {
                            n.AllSentHandle.WaitOne(3000); // wait until all is sent
                        }
                        catch (TimeoutException)
                        {
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Timeout");
                            Dispatcher.Invoke(addMessage, "Timeout");
                            continue;
                        }

                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "'Autentization' sent");
                        Dispatcher.Invoke(addMessage, "Handshake sent");

                        bool disconnect = false;
                        Authentication auth = null;
                        int noneMessageNo = 0;

                        while (startGameMessageReceived == false && disconnect == false && n.IsInitialized == true)
                        {
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Start async receiving");
                            n.ReceiveAsync();
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Waiting for a message to be received");

                            try
                            {
                                n.ReceivedMessageHandle.WaitOne(2500);
                            }
                            catch (TimeoutException)
                            {
                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Timeout");
                                Dispatcher.Invoke(addMessage, "Timeout");
                                break;
                            }

                            
                            NetworkMessageType messageType = n.GetReceivedMessageType();
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Received message: " + messageType);
                            object message = null;

                            if (messageType != NetworkMessageType.None)
                            {
                                noneMessageNo = 0;
                                message = n.GetReceivedMessageFromQueue();
                            }


                            if (messageType == NetworkMessageType.Authentication)
                            {
                                auth = (Authentication)message;
                                DebuggerIX.WriteLine(DebuggerTag.Net, role,
                                    string.Format("'Autentization' message details: {0}, {1}", auth.Name, auth.IP));                                

                                Dispatcher.Invoke(addMessage, "Autentization message received");
                                
                            }
                            else if (messageType == NetworkMessageType.DisconnectRequest)
                            {
                                DisconnectRequest dr = (DisconnectRequest)message;
                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "'DisconnectRequest' was sent by the other side in: " +
                                    dr.DateTime);

                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Sending 'DisconnectRequestConfirmation' message");
                                n.SendAsync(NetworkMessageType.DisconnectRequestConfirmation);
                                n.AllSentHandle.WaitOne();
                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "'DisconnectRequestConfirmation' sent");
                                disconnect = true;
                                Dispatcher.Invoke(addMessage, "Send request to end handshake");
                                
                            }
                            else if (messageType == NetworkMessageType.StartGame)
                            {
                                startGameMessageReceived = true;
                            }
                            else if (messageType == NetworkMessageType.None)
                            {
                                noneMessageNo++;

                                if (noneMessageNo > 2)
                                {
                                    Dispatcher.Invoke(addMessage, "No message was received in three attempts in a row. Giving up.");
                                    break;
                                }
                                else
                                {
                                    Dispatcher.Invoke(addMessage, "No message was received.");
                                }
                            }
                            else
                            {
                                Dispatcher.Invoke(addMessage, "Warning: Unknown message received.");
                            }
                        }

                        if (n.IsInitialized == false)
                        {
                            Dispatcher.Invoke(addMessage, "Connection lost.");
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Connection lost.");
                        }

                        if (disconnect)
                        {
                            n.CloseConnection();
                            Dispatcher.Invoke(addMessage, "End of initial handshake");
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Connection closed.");
                        }

                        if (startGameMessageReceived && auth != null)
                        {
                            wasGameStarted = true;
                            Dispatcher.Invoke(addMessage, "Game is about to start.");                            
                            Dispatcher.Invoke(new EstablishConnectionDelegate(establishConnection),
                                new object[] { n, auth });
                            break; // from outer while cyclus
                        }
                    }

                    i++;
                }

                if (wasGameStarted == false)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "All attempts tried. Click 'Again' to begin again.");
                    Dispatcher.Invoke(addMessage, "All attempts tried. Click 'Again' to begin again.");
                }
                else
                {
                    Dispatcher.BeginInvoke(threadStoppedDel, new object[] { wasGameStarted });
                }
            }            
            catch (ThreadInterruptedException e)
            {
                n.CloseConnection();
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "Thread interrupted. Message: " + e.Message);
                Dispatcher.Invoke(addMessage, "Connection attempts aborted by user request.");
            }
            finally
            {

                if (wasGameStarted == false)
                {
                    n.CloseConnection();
                    Dispatcher.Invoke(addMessage, "Network connection searching was disabled.");
                }                
            }
            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Finished");
        }

        private void threadStopped(bool isGameStarted)
        {
            if (isGameStarted == true)
            {
                t1.Join(); 
                this.Close(); // hide the dialog after successful autentication
            }
        }

        private void btnAgain_Click(object sender, RoutedEventArgs e)
        {
            Status = "";
            this.cancel();

            if (t1 != null && t1.ThreadState != ThreadState.Stopped)
            {
                Status = "Cannot start connecting process. Please wait a few seconds.";
            }
            else
            {
                this.start();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ThreadClientParam
    {
        public Authentication Authentication { get; set; }

        public ThreadClientParam(Authentication auth)
        {
            this.Authentication = auth;
        }
    }
}