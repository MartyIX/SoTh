//#define DEBUG_elapsed_time

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using System.Timers;
using Sokoban.Lib;
using System.Threading;

namespace Sokoban.Model
{
    using Sokoban = Sokoban.Model.GameDesk.Sokoban;
    using Sokoban.Lib.Events;    

    public partial class GameRepository : IBaseRepository
    {
        public AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        
        private Thread processingThread;
        private bool endProcessingThread = false;
        private Event? ud;
        private Int64 lastProcessedTime = 0;
        private double phase = 0;

        public void WakeUpProcessingThread()
        {
            if (processingThread == null)
            {
                processingThread = new Thread(new ThreadStart(Process));
            }

            if (processingThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                processingThread.Start();
            }

            // Resume thread
            autoResetEvent.Set();
        }

        public void DisposeModel()
        {
            if (processingThread != null)
            {
                endProcessingThread = true;
                autoResetEvent.Set();
            }
        }

        /// <summary>
        /// The main method of simulation - reads calendar and processes events.
        /// </summary>
        /// <param name="forceProcessing">Process events even if game is not running</param>
        public void Process()
        {
            while (autoResetEvent.WaitOne())
            {
                // Terminate thread
                if (endProcessingThread == true) break;
                this.ProcessAllEvents();
            }
        }

        public void ProcessAllEvents()
        {
            ProcessAllEvents(true, 0);
        }

        public void ProcessAllEvents(double phase)
        {
            ProcessAllEvents(true, phase);
        }

        public void ProcessAllEvents(bool updateTime, double phase)
        {
            if (gameState == GameState.Running)
            {
                this.phase = phase;

                if (updateTime)
                {
                    Interlocked.Increment(ref time);
                }

                if (calendar.CountOfEvents > 0)
                {
                    Debugger.WriteLine("[PreProcessAllEvents]", "=== Time: " + time.ToString() + " ===");
                }
                else
                {
                    Debugger.WriteLine("[PreProcessAllEvents]", "=== Time: " + time.ToString() + " === | Empty calendar");
                }

                while ((ud = calendar.First(time)) != null)
                {
                    //if (IsPermitted[ud.what])
                    //{
                    ProcessEvent(ud);

                    ud.Value.who.MovementEventsInBuffer -= 
                        EventTypeLib.IsEventOfType(ud.Value.what, EventCategory.goXXX) ? 1 : 0;

                    if (ud.Value.who == pSokoban)
                    {
                        Debugger.WriteLine("[GR-ProcessAllEvents]", ud.ToString());
                    }
                }

                lastProcessedTime = time;
            }
        }


        public void IncrementSimulationTime()
        {
            Interlocked.Increment(ref time);
        }

        public Int64 GetSimulationTime()
        {
            return Interlocked.Read(ref time);
        }
    }
}
