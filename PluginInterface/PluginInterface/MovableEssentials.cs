using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Lib.Events;

namespace Sokoban.Model.PluginInterface
{
    public class MovableEssentials : IMovable
    {
        private object syncRoot = new object();
        protected IPluginParent host;
        protected int lastPosX;
        protected int lastPosY;
        protected int posX;
        protected int posY;

        public MovableEssentials(IPluginParent host)
        {
            this.host = host;
        }

        public virtual void PrepareMovement(Int64 goTime, double phase, IGamePlugin who, EventType ev)
        {
            IMovable gamePlugin = who as IMovable;

            if (gamePlugin != null)
            {
                // Change X, Y coordinates
                gamePlugin.MakeMove((MovementDirection)ev);
                gamePlugin.MovementStartTime = goTime;
                gamePlugin.MovementStartTimePhase = phase;
                gamePlugin.MovementEndTime = goTime + gamePlugin.Speed;
                gamePlugin.LastMovementEvent = ev;

                if (EventTypeLib.IsEventOfType(ev, EventCategory.goXXX))
                {
                    gamePlugin.MovementEventsInBuffer += 1;
                    gamePlugin.TimeMovementEnds = goTime + gamePlugin.Speed;
                }
            
                host.MakePlan("PrepMovWentXXX", gamePlugin.MovementEndTime, who, (EventType)((int)ev + 10)); // goXXX -> wentXXX
            }
        }


        #region IMovable Members

        private int speed; 
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        /// <summary>
        /// Moves GameObject on game desk
        /// </summary>
        /// <param name="whereTo">which direction should GameObject move</param>
        public void MakeMove(MovementDirection whereTo)
        {
            lastPosX = posX;
            lastPosY = posY;

            if (whereTo == MovementDirection.goLeft)
            {
                posX--;
            }
            else if (whereTo == MovementDirection.goRight)
            {
                posX++;
            }
            else if (whereTo == MovementDirection.goUp)
            {
                posY--;
            }
            else if (whereTo == MovementDirection.goDown)
            {
                posY++;
            }
        }

        protected MovementDirection direction;

        /// <summary>
        /// Direction to which the object is turned to
        /// </summary>
        public MovementDirection Direction
        {
            get
            {
                lock (syncRoot)
                {
                    return direction;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    direction = value;
                }
            }
        }

        protected Int64 movementStartTime;        

        public Int64 MovementStartTime
        {
            get
            {
                lock (syncRoot)
                {
                    return movementStartTime;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    movementStartTime = value;
                }
            }
        }

        protected double movementStartTimePhase;

        public double MovementStartTimePhase
        {
            get
            {
                lock (syncRoot)
                {
                    return movementStartTimePhase;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    movementStartTimePhase = value;
                }
            }
        }

        protected Int64 movementEndTime;

        public Int64 MovementEndTime
        {
            get
            {
                lock (syncRoot)
                {
                    return movementEndTime;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    movementEndTime = value;
                }
            }
        }

        protected EventType lastMovementEvent;

        public EventType LastMovementEvent
        {
            get
            {
                lock (syncRoot)
                {
                    return lastMovementEvent;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    lastMovementEvent = value;
                }
            }
        }

        protected int movementEventsInBuffer = 0;

        public int MovementEventsInBuffer 
        { 
            get { return movementEventsInBuffer; }
            set { movementEventsInBuffer = value; }
        }

        protected Int64 timeMovementEnds = 0; 

        public Int64 TimeMovementEnds { 
            get { return timeMovementEnds; }
            set { timeMovementEnds = value; } 
        }

        public int PosX
        {
            get { return posX; }
            set { posX = value; } 
        }

        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        #endregion

    }
}
