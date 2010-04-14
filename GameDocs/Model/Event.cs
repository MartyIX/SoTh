using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using Sokoban.Lib;
using Sokoban.Model;
using Sokoban.Model.GameDesk;

namespace Sokoban
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
        public GameObject who;

        /// <summary>
        /// Type of event
        /// </summary>
        public EventType what;

        /// <summary>
        /// For synchronization (in single player it's not necessary)
        /// </summary>
        public int posX;
        public int posY;
        public int EventID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ID">Identifier of the event - for debugging purposes</param>
        /// <param name="when">When to process the event</param>
        /// <param name="who">Which object should process the event</param>
        /// <param name="what">Type of event</param>
        public Event(int ID, Int64 when, GameObject who, EventType what)
        {
            this.EventID = ID;
            this.when = when;
            this.who = who;
            this.what = what;
            // useful for two player game
            this.posX = who.posX;
            this.posY = who.posY;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="when">When to process the event</param>
        /// <param name="who">Which object should process the event</param>
        /// <param name="what">Type of event</param>
        public Event(int ID, Int64 when, GameObject who, EventType what, int posX, int posY)
        {
            this.EventID = ID;
            this.when = when;
            this.who = who;
            this.what = what;
            this.posX = posX;
            this.posY = posY;
        }

        public bool IsIdenticalWith(Int64 when, GameObject who, EventType what)
        {
            return (this.who == who && this.when == when && this.what == what);
        }

        public override string ToString()
        {
            if (posX != -1)
            {
                return "EID = " + EventID.ToString().PadRight(4) 
                    + "; Time = " + when.ToString().PadRight(5) 
                    + "; Obj = " + who.Description 
                    + "; Ev = " + what.ToString().PadRight(15) 
                    + "; Pos:" + posX.ToString().PadLeft(2) 
                    + "x" + posY.ToString();
            }
            else
            {
                return "EID = " + EventID.ToString().PadRight(4) 
                    + "; Time = " + when.ToString().PadRight(5) 
                    + "; Obj = " + who.Description 
                    + "; Ev = " + what.ToString().PadRight(15) 
                    + "; Pos: " + who.posX.ToString().PadLeft(2) 
                    + "x" + who.posY.ToString();
            }
        }
    }

}