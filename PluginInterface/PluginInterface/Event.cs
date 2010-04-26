using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Diagnostics;
using Sokoban.Lib;
using Sokoban.Model;
using Sokoban.Model.PluginInterface;

namespace Sokoban.Model.PluginInterface
{
    /// <summary>
    /// Event for calendar in discrete simulation of round
    /// </summary>
    public struct Event
    {
        /// <summary>
        /// When to process the event
        /// </summary>
        public Int64 when;

        /// <summary>
        /// Which object should process the event
        /// </summary>
        public IGamePlugin who;

        /// <summary>
        /// Type of event
        /// </summary>
        public EventType what;

        /// <summary>
        /// For synchronization (in single player it's not necessary)
        /// </summary>
        public int EventID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ID">Identifier of the event - for debugging purposes</param>
        /// <param name="when">When to process the event</param>
        /// <param name="who">Which object should process the event</param>
        /// <param name="what">Type of event</param>
        public Event(int ID, Int64 when, IGamePlugin who, EventType what)
        {
            this.EventID = ID;
            this.when = when;
            this.who = who;
            this.what = what;
            // useful for two player game
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="when">When to process the event</param>
        /// <param name="who">Which object should process the event</param>
        /// <param name="what">Type of event</param>
        public Event(int ID, Int64 when, IGamePlugin who, EventType what, int posX, int posY)
        {
            this.EventID = ID;
            this.when = when;
            this.who = who;
            this.what = what;
        }

        public bool IsIdenticalWith(Int64 when, IGamePlugin who, EventType what)
        {
            return (this.who == who && this.when == when && this.what == what);
        }

        public override string ToString()
        {
            IMovableElement obj = who as IMovableElement;

            return "EID = " + EventID.ToString().PadRight(4)
                + "; Time = " + when.ToString().PadRight(5)
                + "; Obj = " + who.Description
                + "; Ev = " + what.ToString().PadRight(15)
                + (obj != null ?
                        "; Pos = " + obj.PosX.ToString().PadLeft(2) + "x" + obj.PosY.ToString()
                    + "; TME = " + obj.TimeMovementEnds.ToString() : ""
                    );
        }
    }

}