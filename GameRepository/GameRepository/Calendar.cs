//#define DEBUG_use_priority_queue

#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using Sokoban.Lib;
using Sokoban.Model;
using Sokoban.Model.GameDesk;
using Sokoban.Lib.Events;
using Sokoban.Model.PluginInterface;
#endregion usings

namespace Sokoban
{
    public delegate void SimulationEventHandler(int eventID, int objectID, Int64 when, EventType what);

    /// <summary>
    /// Queue of events of discrete simulation
    /// </summary>
    public class Calendar
    {
        private int eventCounter = 1;
        private Int64 lastEventTimeInCalendar;
        private bool isEnabledAddingEvents = true;
        private bool isEnabledAddingMovementEvents = true;

        public bool IsEnabledAddingMovementEvents
        {
            get { return isEnabledAddingMovementEvents; }
            set { isEnabledAddingMovementEvents = value; }
        }

        public bool IsEnabledAddingEvents
        {
            get { return isEnabledAddingEvents; }
            set { isEnabledAddingEvents = value; }
        }

        public Int64 LastEventTimeInCalendar
        {
            get { return lastEventTimeInCalendar; }
        }

        #if DEBUG_use_priority_queue
        private PriorityQueue<Event, Int64> list;
        #else
        private List<Event> list;
        #endif

        // one-based
        /// <summary>
        /// Constructor.
        /// </summary>
        public Calendar()
        {
            #if DEBUG_use_priority_queue        
                list = new PriorityQueue<Event, Int64>(new PriorityQueueComparer());
            #else
                list = new List<Event>();                
            #endif            
        }

        public int CountOfEvents { get { return list.Count; } }

        public event SimulationEventHandler AddedEvent;

        /// <summary>
        /// Adds new event to the calendar.
        /// </summary>
        /// <param name="when">Time when event should be processed. It's possible to add two events with same time (order of execution is not defined).</param>
        /// <param name="who">Which object should do the event</param>
        /// <param name="what">What event should be done</param>
        public Event? AddEvent(Int64 when, IGamePlugin who, EventType what)
        {
            return AddEvent(when, who, what, false);
        }

        /// <summary>
        /// Function adds an event to the calendar
        /// </summary>
        /// <param name="when"></param>
        /// <param name="who"></param>
        /// <param name="what"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="ignoreDisabledAdding"></param>
        /// <returns>New event if event was added to the calendar; otherwise null reference</returns>
        public Event? AddEvent(Int64 when, IGamePlugin who, EventType what, bool ignoreDisabledAdding)
        {
            // no adding of events?
            if (ignoreDisabledAdding == false && isEnabledAddingEvents == false) return null;

            // no adding of movement events?
            if (ignoreDisabledAdding == false && 
                isEnabledAddingMovementEvents == false && 
                EventTypeLib.IsEventOfType(what, EventCategory.movement)) return null;
            
            if (when > lastEventTimeInCalendar) lastEventTimeInCalendar = when;

            Event? newOne = null;

            lock (list)
            {
                // check if there's not the same event in the same time
                newOne = new Event(eventCounter, when, who, what);
                
                eventCounter++;
                #if DEBUG_use_priority_queue
                    list.Enqueue(newOne, newOne.when);
                #else
                   list.Add(newOne.Value);
                #endif                                            

                if (AddedEvent != null) AddedEvent(eventCounter, who.ID, when, what); //Invoking all the event handlers
            }
            
            return newOne;
        }

        /// <summary>
        /// First event to process in calendar
        /// </summary>
        /// <returns>An event with minimal time in the calendar but time of the event can't be greater than @actualTime</returns>
        public Event? First(Int64 actualTime)
        {
            Event? first = null;

            if (list.Count > 0)
            {
                lock (list)
                {
                    #if DEBUG_use_priority_queue        
                                                       
                        Event ev = list.Peek().Value;
                        if (ev.when <= actualTime) first = list.Dequeue().Value;

                    #else
                        foreach (Event ev in list)
                        {
                            if (ev.when <= actualTime && ((first == null) ||
                                (first != null && ev.when < first.Value.when)))
                            {
                                first = ev;
                            }
                        }

                        if (first != null) list.Remove(first.Value);
                    #endif
                }
            }

            return first;
        }
    }

    class PriorityQueueComparer : IComparer<Int64>
    {
        #region IComparer<long> Members

        /// <summary>
        /// Comparer that returns a > b changes to  b > a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int IComparer<long>.Compare(long a, long b)
        {
            if (a > b) return -1;
            else if (a == b) return 0;
            else return 1;
        }

        #endregion
    }
}