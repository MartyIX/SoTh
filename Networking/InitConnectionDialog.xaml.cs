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
using System.Net;
using Sokoban.Interfaces;
using Sokoban.Lib.Http;
using Sokoban.Cryptography;
using Sokoban.Model.Xml;

namespace Sokoban.View.SetupNetwork
{
    /// <summary>
    /// Interaction logic for ChooseConnectionWindow.xaml
    /// </summary>
    public partial class InitConnection : Window, INotifyPropertyChanged
    {

        // 
        // API
        //

        public string Status
        {
            get { return _status; }
            set { _status = value; Notify("Status"); }
        }

        public bool AutomaticFirstConnect
        {
            get { return automaticFirstConnect; }
            set { automaticFirstConnect = value; Notify("AutomaticFirstConnect"); }
        }

        public int Port
        {
            get { return port; }
            set { port = value; Notify("Port"); }
        }

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; Notify("IpAddress"); }
        }


        public string BtnListenContent
        {
            get { return btnListenContent; }
            set { btnListenContent = value; Notify("BtnListenContent"); }
        }

        public ObservableCollection<Authentication> ConnectingPlayers
        {
            get { return connectingPlayers; }
        }

        public const int MAX_PLAYERS_TO_WAIT_FOR = 3;

        //
        // Private fields
        //

        private delegate void AddAutentizationDelegate(Authentication autentization);
        private delegate void AddMessageDelegate(string message);
        private delegate void EstablishConnectionDelegate(IConnection connection, Authentication authentication);
        private string _status;
        private ObservableCollection<Authentication> connectingPlayers = new ObservableCollection<Authentication>();
        private int port = 56726;
        private string ipAddress;
        private Thread t1;
        private int endTheThread = 0;
        private string btnListenContent = "Listen";
        private int leaguesID;
        private int roundsID;
        private IProfileRepository profile = null;
        private IErrorMessagesPresenter errorPresenter = null;
        private PendingGamesXmlServerResponse response = null;
        private bool automaticFirstConnect = true;
        private IConnectionRelayer connectionRelayer = null;
        private Authentication auth = null;
        private IGameMatch gameMatch = null;

        public InitConnection(int leaguesID, int roundsID, IProfileRepository profileRepository, 
            IErrorMessagesPresenter errorPresenter, IConnectionRelayer connectionRelayer,
            IGameMatch gameMatch)
        {
            InitializeComponent();
            this.DataContext = this;
            IpAddress = "Automatic";
            this.leaguesID = leaguesID;
            this.roundsID = roundsID;
            this.profile = profileRepository;
            this.errorPresenter = errorPresenter;
            this.connectionRelayer = connectionRelayer;
            this.gameMatch = gameMatch;

            auth = new Authentication(profileRepository.Username, profileRepository.IPAddress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">i.e., "/remote/GetInitLeagues/"</param>
        /// <returns>Response of the server</returns>
        private string getRequestOnServer(string request)
        {
            if (profile == null) throw new UninitializedException("Server name was not initialized.");

            string output;

            if (profile.Server == String.Empty)
            {
                output = "error";
                this.Status = "User is not logged in.";
                errorMessage(ErrorMessageSeverity.Low, "User is not logged in. Please log in and click on 'Refresh'");
            }
            else
            {
                string url = profile.Server.TrimEnd(new char[] { '/' }) + request;

                try
                {
                    output = HttpReq.GetRequest(url, "");
                }
                catch (WebException e)
                {
                    output = "error";
                    this.Status = "Error in communication with the server. Please try again in a while.";
                    errorMessage(ErrorMessageSeverity.Medium, "Error in communication with the server. Additional information: " + e.Message);
                }
                catch (Exception e)
                {
                    output = "error";
                    this.Status = "Unknown error occured. Please try again in a while.";
                    errorMessage(ErrorMessageSeverity.High, "Unknown error in communication with the server. Additional information: " + e.Message);
                }

            }

            return output;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                connectionRelayer.Connect(connection, gameMatch, autentization, leaguesID, roundsID);
            }
        }

        private void AddPlayer(Authentication autentization)
        {
            if (!ConnectingPlayers.Contains(autentization))
            {
                ConnectingPlayers.Add(autentization);
            }
        }

        private void AddMessage(string message)
        {
            tbStatus.Text = tbStatus.Text + message + "\n";
        }

        public void ListenProcessing()
        {
            if (btnListenContent == "Listen")
            {
                addGameOffer();
            }
            else
            {
                cancel();
                deleteGameOffer();
            }

        }

        private void addGameOffer()
        {
            BtnListenContent = "Listen";
            bool success = true;
            try
            {
                start();
            }
            catch (ThreadStateException e)
            {
                success = false;
            }

            if (success)
            {
                string preHash = roundsID.ToString() + profile.SessionID + Hashing.CalculateSHA1(profile.Password, Encoding.Default).ToLower();
                DebuggerIX.WriteLine("[Net]","AddGameOffer","preHash: " + preHash);
                string hash = Hashing.CalculateSHA1(preHash, Encoding.Default).ToLower();
                DebuggerIX.WriteLine("[Net]", "AddGameOffer", "Hash: " + hash);
                string httpReq = "/remote/AddGameOffer/?rounds_id=" + roundsID + "&session_id=" + profile.SessionID + "&port=" + this.port + "&hash=" + hash;
                DebuggerIX.WriteLine("[Net]", "AddGameOffer", "HttpReq: " + httpReq);
                string output = getRequestOnServer(httpReq);

                if (output != "error")
                {
                    response = new PendingGamesXmlServerResponse();

                    try
                    {
                        response.Parse(output, PendingGamesXmlServerResponse.ActionType.Add);
                        this.Status = "Initial leagues loaded.";
                    }
                    catch (InvalidStateException e)
                    {
                        this.Status = e.Message;
                        response = null;
                    }

                    if (response == null)
                    {
                        MessageBox.Show("Cannot parse game server answer.");
                        cancel();
                    }
                    else
                    {
                        if (response.Success == true)
                        {
                            this.Status = "Game offer added.";
                            BtnListenContent = "Cancel";
                        }
                        else
                        {
                            MessageBox.Show("Game offer could not be added on the game server.");
                            errorMessage(ErrorMessageSeverity.Medium,
                                "Game offer was not added to the game server. Server's error message:" + response.ErrorMessage);
                            response = null;
                            cancel();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Game server returned error flag. Cannot continue.");
                    cancel();
                }

            }
            else
            {
                MessageBox.Show("There was an error in initialization of network connection.");
            }
        }

        private void deleteGameOffer()
        {
            cancel();
            
            if (response != null && response.OfferID != -1)
            {
                string hash = Hashing.CalculateSHA1(response.OfferID.ToString() +
                    profile.SessionID + Hashing.CalculateSHA1(profile.Password, Encoding.Default).ToLower(), Encoding.Default).ToLower();
                string output = getRequestOnServer("/remote/DeleteGameOffer/?offer_id=" + response.OfferID +
                    "&session_id=" + profile.SessionID + "&hash=" + hash);

                if (output != "error" && output != String.Empty)
                {
                    response = new PendingGamesXmlServerResponse();

                    try
                    {
                        response.Parse(output, PendingGamesXmlServerResponse.ActionType.Delete);
                    }
                    catch (InvalidStateException e)
                    {
                        this.Status = e.Message;
                        response = null;
                    }
                    
                    if (response == null)
                    {
                        MessageBox.Show("Cannot parse game server answer.");
                    }
                    else
                    {
                        if (response.Success == true)
                        {
                            this.Status = "Game offer deleted.";
                            BtnListenContent = "Listen";
                        }
                        else
                        {
                            MessageBox.Show("Game offer could not be deleted from the game server.");
                            errorMessage(ErrorMessageSeverity.Medium, 
                                "Game offer was not deleted from the game server. Server's error message:" + response.ErrorMessage);
                        }                       
                    }
                }
                else
                {
                    MessageBox.Show("Game server returned error flag. Game offer was not deleted.");
                    BtnListenContent = "Listen";
                }
            }
            else
            {
                BtnListenContent = "Listen";
            }
        }


        public void Terminate()
        {
            cancel();
            deleteGameOffer();
        }

        private void start()
        {
            if (t1 == null || t1.ThreadState == ThreadState.Stopped)
            {
                //  Create  a  ThreadStart  instance  using  your  method  as  a  delegate:
                ParameterizedThreadStart methodDelegate = new ParameterizedThreadStart(runServer);
                //  Create  a  Thread  instance  using  your  delegate  method:
                t1 = new Thread(methodDelegate);
            }

            if (t1.ThreadState == ThreadState.Unstarted)
            {
                endTheThread = 0;
                ConnectingPlayers.Clear();
                tbStatus.Text = "";

                //  Start  the  thread

                ThreadServerParam param = new ThreadServerParam(automaticFirstConnect, auth);

                t1.Start(param);

                disableControls();
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

            if (t1 == null || t1.ThreadState == ThreadState.Stopped)
            {
                enableControls(); 
            }
        }

        private void enableControls()
        {
            tbPort.IsEnabled = true;
            cbAutomaticFirstConnect.IsEnabled = true;
            tbIpAddress.IsEnabled = true;
        }

        private void disableControls()
        {
            tbPort.IsEnabled = false;
            cbAutomaticFirstConnect.IsEnabled = false;
            tbIpAddress.IsEnabled = false;
        }

        private void threadStopped()
        {
            enableControls();
        }

        private void runServer(object obj)
        {
            
            ThreadServerParam param = obj as ThreadServerParam;

            string role = "Server";
            NetworkServer n = new NetworkServer(this.ipAddress, this.port);

            int maxPlayersToWaitFor = MAX_PLAYERS_TO_WAIT_FOR;
            int i = 1;
            AddAutentizationDelegate addPlayer = new AddAutentizationDelegate(AddPlayer);
            AddMessageDelegate addMessage = new AddMessageDelegate(AddMessage);
            VoidChangeDelegate threadStoppedDel = new VoidChangeDelegate(threadStopped);

            if (this.ipAddress == "Automatic")
            {
                Dispatcher.Invoke(addMessage, "Listening on port: " + this.port);
            }
            else
            {
                Dispatcher.Invoke(addMessage, "Listening on: " + this.ipAddress + ":" + this.port);
            }

            bool isAutenticated = false;

            try
            {
                while (i <= maxPlayersToWaitFor && endTheThread != 1)
                {
                    isAutenticated = false;
                    DebuggerIX.WriteLine("[Net]", role, "Initialization # " + i + " invoked");
                    Dispatcher.Invoke(addMessage, "Attempt to receive connection #" + i);

                    try
                    {
                        n.InitializeConnection();
                    }
                    catch (InvalidStateException e)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "InvalidState: " + e.Message);
                        Dispatcher.Invoke(addMessage, "Error:" + e.Message);
                        n.CloseConnection();
                    }
                    catch (TimeoutException e)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "TimeOut: " + e.Message);
                        Dispatcher.Invoke(addMessage, "Timeout for attempt #" + i);
                        n.CloseConnection();
                    }

                    DebuggerIX.WriteLine("[Net]", role, "Initialization = " + n.IsInitialized);

                    if (n.IsInitialized == true)
                    {
                        DebuggerIX.WriteLine("[Net]", role, "Sending 'Autentization'");
                        n.SendAsync(NetworkMessageType.Authentication, param.Authentication);
                        n.AllSentHandle.WaitOne(); // wait until all is sent
                        DebuggerIX.WriteLine("[Net]", role, "'Autentization' sent");
                        Dispatcher.Invoke(addMessage, "Handshake sent");
                        
                        bool disconnect = false;
                        Authentication auth = null;

                        while (isAutenticated == false && disconnect == false)
                        {
                            DebuggerIX.WriteLine("[Net]", role, "Start async receiving");
                            n.ReceiveAsync();
                            DebuggerIX.WriteLine("[Net]", role, "Waiting for a message to be received");
                            n.ReceivedMessageHandle.WaitOne(5000);

                            NetworkMessageType messageType = n.GetReceivedMessageType();
                            DebuggerIX.WriteLine("[Net]", role, "Received message: " + messageType);

                            if (messageType == NetworkMessageType.Authentication)
                            {
                                auth = (Authentication)n.GetReceivedMessageFromQueue();
                                DebuggerIX.WriteLine("[Net]", role,
                                    string.Format("'Autentization' message details: {0}, {1}", auth.Name, auth.IP));

                                Dispatcher.Invoke(addPlayer, auth);
                                Dispatcher.Invoke(addMessage, "Autentization message received");

                                if (param.AutomaticFirstConnect)
                                {
                                    Dispatcher.Invoke(addMessage, "Accepting game opponent.");
                                    isAutenticated = true;

                                    DebuggerIX.WriteLine("[Net]", role, "Sending 'StartGame' message");
                                    n.SendAsync(NetworkMessageType.StartGame);
                                    n.AllSentHandle.WaitOne();
                                    DebuggerIX.WriteLine("[Net]", role, "'StartGame' sent");                                
                                }                            
                            }
                            else if (messageType == NetworkMessageType.DisconnectRequest)
                            {
                                DisconnectRequest dr = (DisconnectRequest)n.GetReceivedMessageFromQueue();
                                DebuggerIX.WriteLine("[Net]", role, "'DisconnectRequest' was sent by the other side in: " +
                                    dr.DateTime);

                                DebuggerIX.WriteLine("[Net]", role, "Sending 'DisconnectRequestConfirmation' message");
                                n.SendAsync(NetworkMessageType.DisconnectRequestConfirmation);
                                n.AllSentHandle.WaitOne();
                                DebuggerIX.WriteLine("[Net]", role, "'DisconnectRequestConfirmation' sent");
                                disconnect = true;
                                Dispatcher.Invoke(addMessage, "Send request to end handshake");
                            }
                        }

                        if (disconnect)
                        {
                            n.CloseConnection();
                            Dispatcher.Invoke(addMessage, "End of initial handshake");
                            DebuggerIX.WriteLine("[Net]", role, "Connection closed.");
                        }

                        if (isAutenticated)
                        {
                            Dispatcher.Invoke(new EstablishConnectionDelegate(establishConnection), 
                                new object[] { n, auth});
                            break; // from outer while cyclus
                        }
                    }

                    i++;
                }

                if (isAutenticated == false)
                {
                    Dispatcher.Invoke(addMessage, "All attempts tried. Click 'listen' to begin again.");
                }
            }
            catch (ThreadInterruptedException e)
            {
                DebuggerIX.WriteLine("[Net]", role, "Thread interrupted.");
                n.CloseConnection();
            }
            finally
            {

                if (isAutenticated == false)
                {
                    n.CloseConnection();
                }

                Dispatcher.Invoke(addMessage, "Network connection searching was disabled.");
            }

            DebuggerIX.WriteLine("[Net]", role, "Finished");
            Dispatcher.Invoke(threadStoppedDel);
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

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            this.ListenProcessing();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Terminate() is in Closing event
        }

    }

    public class ThreadServerParam
    {
        public bool AutomaticFirstConnect { get; set; }
        public Authentication Authentication {get; set; }

        public ThreadServerParam(bool automaticFirstConnect, Authentication auth)
        {
            this.Authentication = auth;
            this.AutomaticFirstConnect = automaticFirstConnect;
        }
    }
}