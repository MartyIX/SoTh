using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using System.Threading;
using Sokoban.Model.PluginInterface;
using Sokoban.Model.GameDesk;

namespace Sokoban.Model
{
   
    public partial class GameRepository
    {
        private Event? ud;
        private double phase = 0;        

        /// <summary>
        /// Processes given event
        /// </summary>
        /// <param name="ev">Event to process</param>
        public void ProcessEvent(Event e)
        {
            // for events that are not handled by plugins

            if (e.what == EventType.gameWon)
            {
                if (GameChanged != null)
                {
                    GameChanged(GameChange.Won);
                }
            }
            else if (e.what == EventType.stopCountingTime)
            {
                if (GameChanged != null)
                {
                    GameChanged(GameChange.StopCountingTime);
                }
            }
            else if (e.what == EventType.gameLost)
            {
                if (GameChanged != null)
                {
                    GameChanged(GameChange.Lost);
                }
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

        private string playerNumber()
        {
            return ((gameDisplayType == GameDisplayType.FirstPlayer) ? "#1" : "#2");
        }

        public void ProcessAllEvents(bool updateTime, double phase)
        {
            this.phase = phase;

            if (updateTime)
            {
                Interlocked.Increment(ref time);
            }

            if (calendar.CountOfEvents > 0)
            {
                DebuggerIX.WriteLine(DebuggerTag.SimulationEventHandling, "[EventProcessing|Player" + playerNumber() + "]", "=== Time: " + time.ToString() + " ===");
            }
            else
            {
                DebuggerIX.WriteLine(DebuggerTag.SimulationEventHandling, "[EventProcessing|Player" + playerNumber() + "]", "=== Time: " + time.ToString() + " === | Empty calendar");
            }

            while ((ud = calendar.First(time)) != null)
            {
                if (ud.Value.who == null ||  ud.Value.who.ProcessEvent(time, ud.Value) == false)
                {
                    this.ProcessEvent(ud.Value);
                }
            }

            gameVariant.CheckRound(time);
        }
    }
}
