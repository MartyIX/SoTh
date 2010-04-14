#region Using statements
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
using System.Xml;
using System.Diagnostics;
#endregion Using statements

namespace Sokoban
{
    /// <summary>
    /// Discrete simulation for actual round
    /// </summary>
     
    public class IsPermitted<T>
    {
        private bool[] isPermitted;

        public IsPermitted()
        {
            isPermitted = new bool[Enum.GetValues(typeof(T)).Length];

            for (int i = 0; i < isPermitted.Length; ++i)
                isPermitted[i] = true;
        }

        /// <summary>
        /// Gets or sets if an EventType is permitted or not to process in this.Move() method
        /// </summary>
        /// <param name="et"></param>
        /// <returns>true if EventType is permitted; otherwise false</returns>
        public bool this[EventType et]
	    {
            get { return isPermitted[(int)et - 1]; }
            set { isPermitted[(int)et - 1] = value; }
	    }
    }

    public class Simulation
    {
		#region Fields (6) 

        public Calendar calendar;
        private GameDeskView form;
        private Player player;
        public Int64 time = 0;
        private Event ud;
        public IsPermitted<EventType> IsPermitted;


		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="form">Reference to main form</param>
        public Simulation(GameDeskView form, Player player)
        {
            this.player = player;
            this.form = form;
            calendar = new Calendar();
            ud = null;

            IsPermitted = new IsPermitted<EventType>();
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Actual time of discrete simulation
        /// </summary>
        public Int64 Time
        {
            get { return time; }
        }

		#endregion Properties 

		#region Methods (4) 

		// Public Methods (4) 

        /// <summary>
        /// Adds event to the calendar of model
        /// </summary>
        /// <param name="when">Time when action should be processed.</param>
        /// <param name="who"></param>
        /// <param name="what">What event should be processed.</param>
        public void MakePlan(Int64 when, GameObject who, EventType what)
        {
            Event ev = calendar.AddEvent(when, who, what);

            if (ev != null)
            {
                GameDeskView.Debug(player.ToString() + " " + ev.ToString(), "Calendar-adding");
            }
        }

        /// <summary>
        /// The main method of simulation - reads calendar and processes events.
        /// </summary>
        public void Move()
        {
            Move(false);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceProcessing">Process events even if game is not running</param>
        public void Move(bool forceProcessing)
        {
            if (forceProcessing == true || player.gameState == GameState.Running)
            {
                while ((ud = calendar.First(time)) != null) 
                {
                    if (IsPermitted[ud.what])
                    {
                        ud.who.ProcessEvent(ud);
                        GameDeskView.Debug(ud.ToString(), "Calendar" + player.ToString());
                    }
                    else
                    {
                        GameDeskView.Debug("[notPermitted] " + ud.ToString(), "Calendar" + player.ToString());
                    }
                }
            }
        }

		#endregion Methods 
    }
}