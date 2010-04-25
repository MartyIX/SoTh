using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;

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
        protected Image image;

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


        protected float lastDrawedX = 0;
        protected Stopwatch redrawStopWatch = new Stopwatch();
        protected ScaleTransform scale;

        public bool MovementInProgress(Int64 time)
        {
            return (time < movementEndTime) ? true : false;
        }

        public virtual void Draw(Canvas canvas, double squareSize, Int64 time, double phase)
        {
            // 50x50 is maximal expected size of image
            double expectedSize = 50;

            double fieldSizeScaleX = image.ActualWidth / expectedSize;
            double fieldSizeScaleY = image.ActualHeight / expectedSize;

            double centerXpos = squareSize * (1 - fieldSizeScaleX) / 2;
            double centerYpos = squareSize * (1 - fieldSizeScaleY) / 2;

            double x;
            double y;

            if (MovementInProgress(time) == false)
            {
                x = (this.posX - 1) * squareSize + centerXpos;
                y = (this.posY - 1) * squareSize + centerYpos;

                DebuggerIX.WriteLine("[Draw]", "NotInProgress",
                        "[x,y] = " + x.ToString("0.00")
                    + ", " + y.ToString("0.00")
                    + "; pos = " + posX.ToString() + "x" + posY.ToString()
                    + "; phase = " + phase.ToString("0.0000"));
            }
            else
            {
                double startTime = (double)movementStartTime + movementStartTimePhase;
                double timePassed = ((double)time + phase - startTime);
                double progress = timePassed / (double)(this.Speed - movementStartTimePhase);
                double step = squareSize * progress;

                x = (this.lastPosX - 1) * squareSize + centerXpos + (posX - lastPosX) * step;
                y = (this.lastPosY - 1) * squareSize + centerYpos + (posY - lastPosY) * step;

                DebuggerIX.WriteLine("[Draw]", "InProgress",
                        "[x,y] = " + x.ToString("0.00")
                    + ", " + y.ToString("0.00")
                    + "; pos = " + lastPosX.ToString() + "x" + lastPosY.ToString()
                    + " -> " + posX.ToString() + "x" + posY.ToString()
                    + "; SS = " + squareSize.ToString()
                    + "; MST = " + startTime.ToString("0.000")
                    + "; Time = " + time.ToString() + phase.ToString(".0000")
                    + "; progress = " + progress.ToString("0.000")
                    + "; step = " + step.ToString()
                    + "; TimeMovementEnd = " + movementEndTime.ToString());

            }


            double scaleX = (squareSize * fieldSizeScaleX) / image.ActualWidth;
            double scaleY = (squareSize * fieldSizeScaleY) / image.ActualHeight;

            if (scale == null || (scale.ScaleX != scaleX || scale.ScaleY != scaleY))
            {
                scale = new ScaleTransform(scaleX, scaleY);
                image.RenderTransform = this.scale;
            }

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);

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
