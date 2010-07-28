using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;
using System.Diagnostics;
using Sokoban.Model.PluginInterface;
using System.Windows.Input;

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
    public partial class GameRepository
    {        
        public event VoidChangeDelegate GameStarted;
        public IGameRealTime game = null;
        private bool wasGameStarted = false;

        /// <summary>
        /// Method dispatches info that user pressed key (keydown event) to the plugins that can react on keys
        /// </summary>
        public bool MoveRequest(Key key, double phase)
        {
            if (wasGameStarted == false)
            {
                if (GameStarted != null) GameStarted(); // Fire event
                wasGameStarted = true;
            }
            
            bool wasHandled = false;

            foreach (IControllableByUserInput gp in this.controllableByUserObjects)
            {
                if (gp.OnKeyDown(key, time, phase) == true) // was request handled
                {
                    wasHandled = true;
                }
            }

            return wasHandled;
        }

        /// <summary>
        /// Method dispatches info that user released key (keyup event) to the plugins that can react on keys
        /// </summary>
        public bool StopMove(Key key, double phase)
        {
            bool wasHandled = false;

            foreach (IControllableByUserInput gp in this.controllableByUserObjects)
            {               
                if (gp.OnKeyUp(key, time, phase) == true)
                {
                    wasHandled = true;
                }
            }

            return wasHandled;        
        }
    }
}
