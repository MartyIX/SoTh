using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Networking;
using System.Threading;
using Sokoban.Lib;

namespace NetworkingTest
{
    class Program
    {
        private static ManualResetEvent[] resetEvents;
        private static NetworkServer ns = new NetworkServer(51000);
        private static NetworkClient nc = new NetworkClient("10.0.0.5", 51000);

        static void Main(string[] args)
        {                        
            DebuggerIX.Start(DebuggerMode.OutputWindow);

            resetEvents = new ManualResetEvent[1];
            resetEvents[0] = new ManualResetEvent(false);

            // Send the custom object to the threaded method.
            //ThreadPool.QueueUserWorkItem(new WaitCallback(RunProducer), threadInfo);
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunConsument));
            RunProducer();

            Console.WriteLine("Waiting for ThreadPool thread to finish...");

            WaitHandle.WaitAny(resetEvents);
            
            nc.CloseConnection();
            
            Console.WriteLine("Finished");

            DebuggerIX.Close();
        }

        public static void RunProducer()
        {
            DebuggerIX.WriteLine("[Net]", "Producer", "Method invoked");
                       

            int tries = 0;
            while (ns.IsInitialized == false && tries < 3)
            {
                ns.InitializeConnection();
                tries++;
            }

            if (ns.IsInitialized != false)
            {
                DebuggerIX.WriteLine("[Net]", "Producer", "Adding events");
                ns.AddEventToBuffer(1, 1, EventType.none, 1, 1);
                ns.AddEventToBuffer(2, 2, EventType.none, 2, 2);
                DebuggerIX.WriteLine("[Net]", "Producer", "Sending");
                ns.Send(NetworkMessageType.ListOfEvents);
                DebuggerIX.WriteLine("[Net]", "Producer", "Sent");
            }
            ns.CloseConnection();
        }

        public static void RunConsument(object a)
        {
            DebuggerIX.WriteLine("[Net]", "Consument", "Method invoked");

            
            DebuggerIX.WriteLine("[Net]", "Consument", "Initializing");

            int tries = 0;
            while (nc.IsInitialized == false && tries < 3)
            {                
                nc.InitializeConnection();
                tries++;
            }

            if (nc.IsInitialized != false)
            {
                DebuggerIX.WriteLine("[Net]", "Consument", "Receiving");
                nc.Receive();
                DebuggerIX.WriteLine("[Net]", "Consument", "Received");
            }

            resetEvents[0].Set();
        }

    }
}
