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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sokoban;
using Sokoban.Model;
using AvalonDock;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Sokoban.Lib;
using System.Globalization;
using System.Threading;
using Sokoban.View.SetupNetwork;
using Sokoban.Networking;
using Sokoban.Lib.Exceptions;
using DummyObjectImplementations;
using Sokoban.Interfaces;
using Sokoban.Model.PluginInterface;

namespace Sokoban
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IConnectionRelayer
    {
        private InitConnection initConnectionDialog;
        private ConnectionDialog connectionDialog;

        private IConnection conn1 = null;
        private IConnection conn2 = null;

        public Window1()
        {
            Initialize();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            DataContext = this;
        }

        private void Initialize()
        {
            PluginService.Initialize(@"D:\Bakalarka\Sokoban\Main\Plugins");
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {

            if (initConnectionDialog != null)
            {
                initConnectionDialog.Terminate();
                initConnectionDialog.Close();
            }

            initConnectionDialog = new InitConnection(1, 1, new DummyProfileRepository(), null, this, null);
            initConnectionDialog.Show();

        }

        private void btnStartClient_Click(object sender, RoutedEventArgs e)
        {
            if (connectionDialog != null)
            {
                connectionDialog.Terminate();
                connectionDialog.Close();
            }

            DummyProfileRepository profile2 = new DummyProfileRepository();
            profile2.username = "Tester";

            connectionDialog = new ConnectionDialog("10.0.0.5", 56726, 1, 1, profile2, null, this, null);
            connectionDialog.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.Start(DebuggerMode.File);

            loadQuest();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DebuggerIX.Close();
        }

        private void loadQuest()
        {
            //string result = Sokoban.Properties.Resources.TestQuest;
            string result = Sokoban.Properties.Resources.SimpleQuest;
            //gameManager.Add(result);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                loadQuest();
                e.Handled = true;
            }
            else if (e.Key == Key.R)
            {
                gameManager.ActiveGameControl.Reload();
            }
            else if (e.Key == Key.D)
            {

            }
            else
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.KeyIsDown(sender, e);
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            gameManager.ActiveGameControl.KeyIsUp(sender, e);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //testList();
        }

        #region IConnectionRelayer Members

        public void Connect(IConnection connection, IGameMatch gameMatch, Authentication autentization, int leaguesID, int roundsID)
        {
            DebuggerIX.WriteLine(DebuggerTag.Net, "Connected", "Auth: " + autentization.Name + " " + autentization.IP);
            DebuggerIX.WriteLine(DebuggerTag.Net, "Connected", "Game = {LeaguesID: " + leaguesID + "; RoundsID: " + roundsID + "}");

            if (conn1 == null)
            {
                conn1 = connection;
            }
            else
            {
                conn2 = connection;
                runTests();
            }

        }

        #endregion


        public void runTests()
        {
            runTest01();

        }

        public void runTest01()
        {
            List<Event> list = new List<Event>();

            for (int i = 1; i <= 100; i++)
            {
                conn1.AddEventToBuffer(i, i, EventType.wentDown, 1, 1);
            }

            conn1.SendAsync(NetworkMessageType.ListOfEvents);
            conn1.AllSentHandle.WaitOne();

            NetworkMessageType messageType = NetworkMessageType.StartGame;

            while (messageType != NetworkMessageType.None)
            {
                conn2.ReceiveAsync();
                conn2.ReceivedMessageHandle.WaitOne();

                messageType = conn2.GetReceivedMessageType();

                if (messageType == NetworkMessageType.ListOfEvents)
                {
                    object obj = conn2.GetReceivedMessageFromQueue();

                    Queue<NetworkEvent> l = obj as Queue<NetworkEvent>;

                    if (l != null)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, "runTest01", "List was received");
                    }
                    else
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, "runTest01", "Error: list was not received");
                    }
                }
                else
                {
                    conn2.GetReceivedMessageFromQueue();
                }
            }               
        }

    }
}