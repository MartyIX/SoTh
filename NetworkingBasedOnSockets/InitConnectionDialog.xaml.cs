using System;
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
using System.Windows.Shapes;
using Sokoban.Presenter;
using Sokoban.Model;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using Sokoban.Networking;
using Sokoban.Lib;
using System.Threading;
using Sokoban.Lib.Exceptions;

namespace Sokoban.View.SetupNetwork
{
    /// <summary>
    /// Interaction logic for ChooseConnectionWindow.xaml
    /// </summary>
    public partial class InitConnection : Window, INotifyPropertyChanged
    {
        public const int MAX_PLAYERS_TO_WAIT_FOR = 3;
        private ObservableCollection<PlayerInfo> connectingPlayers = new ObservableCollection<PlayerInfo>();

        public ObservableCollection<PlayerInfo> ConnectingPlayers
        {
            get { return connectingPlayers; }
        }

        private Thread t;
        private int endTheThread = 0;

        public InitConnection()
        {
            InitializeComponent();
            this.DataContext = this;

            //  Create  a  ThreadStart  instance  using  your  method  as  a  delegate:
            ThreadStart methodDelegate = new ThreadStart(runThread);
            //  Create  a  Thread  instance  using  your  delegate  method:
            t = new Thread(methodDelegate);            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AddPlayer(PlayerInfo playerInfo)
        {
            if (!ConnectingPlayers.Contains(playerInfo))
            {
                ConnectingPlayers.Add(playerInfo);
            }
        }

        public void Start()
        {
            //  Start  the  thread
            t.Start();
        }

        public void Cancel()
        {
            if (t.ThreadState== ThreadState.Running || t.ThreadState == ThreadState.WaitSleepJoin)
            {
                t.Interrupt();
                if (endTheThread == 0)
                {
                    Interlocked.Increment(ref endTheThread);
                }
            }
        }

        private void runThread()
        {
            string role = "Server";
            NetworkServer ns = new NetworkServer(56726);            

            int maxPlayersToWaitFor = MAX_PLAYERS_TO_WAIT_FOR;
            int i = 1;

            try
            {

                while (i <= maxPlayersToWaitFor && endTheThread != 1)
                {
                    DebuggerIX.WriteLine("[Net]", role, "Initialization # " + i + " invoked");

                    try
                    {
                        ns.InitializeConnection();
                    }
                    catch (InvalidStateException e)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "InvalidState: " + e.Message);
                    }
                    DebuggerIX.WriteLine("[Net]", role, "Initialization = " + ns.IsInitialized);

                    if (ns.IsInitialized != false)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "Sending 'Autentization'");
                        ns.SendAsync(NetworkMessageType.Authentication);
                        ns.AllSentHandle.WaitOne(); // wait until all is sent
                        DebuggerIX.WriteLine("[Net]", role, "'Autentization' sent");

                        bool canDisconnect = false;
                        while (canDisconnect == false)
                        {
                            DebuggerIX.WriteLine("[Net]", role, "Start async receiving");
                            ns.ReceiveAsync();
                            DebuggerIX.WriteLine("[Net]", role, "Waiting for a message to be received");
                            ns.ReceivedMessageHandle.WaitOne();

                            NetworkMessageType messageType = ns.GetReceivedMessageType();
                            DebuggerIX.WriteLine("[Net]", role, "Received message: " + messageType);

                            if (messageType == NetworkMessageType.Authentication)
                            {
                                Autentization auth = (Autentization)ns.GetReceivedMessageFromQueue();
                                DebuggerIX.WriteLine("[Net]", role,
                                    string.Format("'Autentization' messaage details: {0}, {1}", auth.Name, auth.IP));
                            }
                            else if (messageType == NetworkMessageType.DisconnectRequest)
                            {
                                DisconnectRequest dr = (DisconnectRequest)ns.GetReceivedMessageFromQueue();
                                DebuggerIX.WriteLine("[Net]", role, "'DisconnectRequest' was sent by the other side in: " +
                                    dr.DateTime);

                                DebuggerIX.WriteLine("[Net]", role, "Sending 'DisconnectRequestConfirmation' message");
                                ns.SendAsync(NetworkMessageType.DisconnectRequestConfirmation);
                                ns.AllSentHandle.WaitOne();
                                DebuggerIX.WriteLine("[Net]", role, "'DisconnectRequestConfirmation' sent");
                                canDisconnect = true;
                            }
                        }

                        ns.CloseConnection();
                        DebuggerIX.WriteLine("[Net]", role, "Connection closed.");
                    }

                    i++;
                }
            }
            catch (ThreadInterruptedException e)
            {
                DebuggerIX.WriteLine("[Net]", role, "Thread interrupted.");
            }
            finally
            {
                ns.CloseConnection();
            }
            DebuggerIX.WriteLine("[Net]", role, "Finished");
        }

        public void RunConsument()
        {
            string role = "Client";

            NetworkClient nc = new NetworkClient("10.0.0.5", 56726);
            DebuggerIX.WriteLine("[Net]", role, "Initialization invoked");

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    nc.InitializeConnection();

                }
                catch (InvalidStateException e)
                {
                    DebuggerIX.WriteLine("[Net]", role, "InvalidState: " + e.Message);
                }
                DebuggerIX.WriteLine("[Net]", role, "IsInitialized = " + nc.IsInitialized);


                if (nc.IsInitialized != false)
                {
                    DebuggerIX.WriteLine("[Net]", role, "Sending 'Autentization'");
                    nc.SendAsync(NetworkMessageType.Authentication);
                    nc.AllSentHandle.WaitOne(); // wait until all is sent
                    DebuggerIX.WriteLine("[Net]", role, "'Autentization' sent");

                    DebuggerIX.WriteLine("[Net]", role, "Receiving 'autentization'");
                    nc.ReceiveAsync();
                    nc.ReceivedMessageHandle.WaitOne();
                    NetworkMessageType messageType = nc.GetReceivedMessageType();
                    DebuggerIX.WriteLine("[Net]", role, "Received message: " + messageType);

                    if (messageType == NetworkMessageType.Authentication)
                    {
                        Autentization auth = (Autentization)nc.GetReceivedMessageFromQueue();
                        DebuggerIX.WriteLine("[Net]", role,
                            string.Format("'Autentization' message details: {0}, {1}", auth.Name, auth.IP));
                    }

                    DebuggerIX.WriteLine("[Net]", role, "Sending 'DisconnectRequest' message");
                    nc.SendAsync(NetworkMessageType.DisconnectRequest);
                    DebuggerIX.WriteLine("[Net]", role, "Waiting for the message to be sent");
                    nc.AllSentHandle.WaitOne();
                    DebuggerIX.WriteLine("[Net]", role, "'DisconnectRequest' was sent");

                    bool canDisconnect = false;

                    while (canDisconnect == false)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "Start async receiving");
                        nc.ReceiveAsync();
                        DebuggerIX.WriteLine("[Net]", role, "Waiting for a message to be received");
                        nc.ReceivedMessageHandle.WaitOne();

                        messageType = nc.GetReceivedMessageType();
                        DebuggerIX.WriteLine("[Net]", role, "Received message: " + messageType);

                        if (messageType == NetworkMessageType.DisconnectRequestConfirmation)
                        {
                            canDisconnect = true;

                        }
                    }

                    nc.CloseConnection();
                    DebuggerIX.WriteLine("[Net]", role, "Connection closed.");
                }
            }
            DebuggerIX.WriteLine("[Net]", role, "Finished");
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Cancel();
        }
    }

    public class PlayerInfo
    {
        public string Name {get;set;}
        public string IP {get;set;}

        public override string ToString()
        {
            return Name + "(" + IP + ")";
        }
    }

}