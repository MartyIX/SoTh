using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;
using System.Diagnostics;

/*
 * Keyboard buffer for Sokoban
 * 
 * Expected behaviour:
 *  - When Sokoban is not moving then OnKeyDown event must lead to the movement
 *  - When Key is hold down then movement must continue until key is released.
 *  - When Key is pressed for a short time then movement must be added to the calendar.
 *    - Maximally three (arbitrary number; property Count) movement events must be added for future processing.
 *  
 * Solutions:
 *  - Add this behaviour to the GameRepository class
 *    - OnKeyDown
 *      - Start stopwatch
 *      - IF Sokoban is moving THEN
 *        - Change pressed key
 *      - ELSE
 *        - Add movement event to calendar and keep information about pressed key
 *        - Set FLAG AlreadyAdded
 *    - OnKeyUp
 *      - IF user pressed key for less than 100ms (according to stopwatch) AND !AlreadyAdded THEN 
 *        - Check how many movement events is in calendar for Sokoban and 
 *          if less than MaxCount then add the event.
*/

namespace Sokoban.Model
{
    using Sokoban = Sokoban.Model.GameDesk.Sokoban;
    using Debugger = Sokoban.Lib.Debugger;

    public partial class GameRepository : IBaseRepository
    {
        const int MAX_EVENTS_IN_KB = 3; 
        bool moveRequestCancelled = true;
        bool kbAlreadyAdded = false;
        public Game game = null;

        /// <summary>
        /// Sokoban's move request - movement continues until StopMove() is called !!!
        /// </summary>
        /// <param name="ev"></param>
        public void MoveRequest(EventType ev)
        {
            Debugger.WriteLine("[GR-MoveRequest]", ">>> MoveRequest <<<", "Time = " + time.ToString());

            lock (pSokoban)
            {
                pSokoban.heldKeyEvent = ev;

                if (pSokoban.TimeMovementEnds <= time)
                {
                    Debugger.WriteLine("[GR-MoveRequest]", "Time is: " + time.ToString() 
                        + "; phase = " + game.CurrentPhase.ToString() 
                        + "; Movement is not in progress.");

                    moveRequestCancelled = false;
                    // In this moment; events for @time are processed, therefore time + 1
                    //MakePlan("SokStartMov", time + 1, (GameObject)pSokoban, pSokoban.heldKeyEvent);
                    MakePlan("SokStartMov", time, (GameObject)pSokoban, pSokoban.heldKeyEvent);
                    kbAlreadyAdded = true;

                    ProcessAllEvents(false, game.CurrentPhase); // We don't want to update time
                } 
                else if (this.kbGetCount() < MAX_EVENTS_IN_KB)
                {
                    Debugger.WriteLine("[GR-MoveRequest]", "MakePlan for move in time: " + (pSokoban.TimeMovementEnds).ToString());
                    MakePlan("SokKeyBuf", pSokoban.TimeMovementEnds, (GameObject)pSokoban, pSokoban.heldKeyEvent);
                }
            }
        }

        /// <summary>
        /// Don't continue with the movement of Sokoban
        /// </summary>
        public void StopMove()
        {
            /*
            if (!kbAlreadyAdded && this.kbGetCount() < MAX_EVENTS_IN_KB)
            {
                Debugger.WriteLine("[GR-StopMove]", "MakePlan for move in time: " + (pSokoban.TimeMovementEnds).ToString());
                MakePlan(pSokoban.TimeMovementEnds, (GameObject)pSokoban, pSokoban.heldKeyEvent);
            }
            else
            {
                Debugger.WriteLine("[GR-StopMove]", "No movement plans for Sokoban.");
            }*/
           
            pSokoban.heldKeyEvent = EventType.none;
            moveRequestCancelled = true;
            kbAlreadyAdded = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int kbGetCount()
        {
            return pSokoban.MovementEventsInBuffer;
        }

        public void PrepareMovement(Int64 goTime, GameObject who, EventType ev)
        {
            // Change X, Y coordinates
            who.MakeMove((MovementDirection)ev);

            who.UI.MovementStartTime = goTime;
            who.UI.MovementStartTimePhase = phase;
            who.UI.MovementEndTime = goTime + who.Speed;
            who.UI.MovementNumberOfFields = 1;
            who.UI.LastMovementEvent = ev;

            MakePlan("PrepMovWentXXX", who.UI.MovementEndTime, who, (EventType)((int)ev + 10)); // goXXX -> wentXXX
        }

        public void PrepareMovement(Int64 goTime, GameObject who, Event ev)
        {
            PrepareMovement(goTime, who, ev.what);
        }

    }
}
