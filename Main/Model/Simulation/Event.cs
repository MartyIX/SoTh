using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;

namespace Sokoban
{
    public enum EventCategory
    {
        goXXX = 0, // goLeft, goUp, goRight, goDown
        movingXXX, // movingLeft, movingUP,...
        movement
    }

    /// <summary>
    /// Event for calendar in discrete simulation of round
    /// </summary>
    public class Event
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

        public static bool IsEventOfType(EventCategory ec, EventType eventType)
        {
            if ((ec == EventCategory.goXXX &&
                (
                  eventType == EventType.goDown || eventType == EventType.goUp ||
                  eventType == EventType.goLeft || eventType == EventType.goRight)
                )
                ||
                (ec == EventCategory.movingXXX &&
                (
                  eventType == EventType.movingDown || eventType == EventType.movingUp ||
                  eventType == EventType.movingLeft || eventType == EventType.movingRight)
                )
                ||
                (ec == EventCategory.movement && 
                (IsEventOfType(EventCategory.goXXX, eventType) || IsEventOfType(EventCategory.movingXXX, eventType))
                )
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            if (posX != -1)
            {
                return EventID.ToString().PadRight(4) + " " + when.ToString().PadRight(5) + " : " + who.Description + " : " + what.ToString().PadRight(15) + " FPos:" + posX.ToString().PadLeft(2) + "x" + posY.ToString();
            }
            else
            {
                return EventID.ToString().PadRight(4) + " " + when.ToString().PadRight(5) + " : " + who.Description + " : " + what.ToString().PadRight(15) + " NPos: " + who.posX.ToString().PadLeft(2) + "x" + who.posY.ToString();
            }
        }
    }

}