//#define DebugDraw

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace Sokoban.Model.PluginInterface
{
    public class MovableEssentials : IMovableElement
    {
        private object syncRoot = new object();
        protected IPluginParent host;
        protected int lastPosX;
        protected int lastPosY;
        protected int posX;
        protected int posY;
        protected Image image;
        protected int obstructionLevel = 0;
        protected int fieldsX = 0;
        protected int fieldsY = 0;
        private static int[] moves = { -1,  0, 
                                        1,  0, 
                                        0, -1, 
                                        0,  1 };        


        public MovableEssentials(IPluginParent host)
        {
            this.host = host;            
        }

        public virtual void PrepareMovement(Int64 goTime, double phase, IGamePlugin who, EventType ev)
        {
            IMovableElement gamePlugin = who as IMovableElement;

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

        public void SetOrientation(EventType e)
        {
            if (((MovementDirection)e == MovementDirection.goLeft || (MovementDirection)e == MovementDirection.goRight))
            {
                direction = (MovementDirection)e;
            }
        }

        [Conditional("DebugDraw")]
        public void Debug(string category, string subcategory, string message)
        {
            DebuggerIX.WriteLine(category, subcategory, message);
        }

        public virtual void Draw(Canvas canvas, double squareSize, Int64 time, double phase)
        {
            double x;
            double y;

            if (MovementInProgress(time) == false)
            {
                x = (this.posX - 1) * squareSize;
                y = (this.posY - 1) * squareSize;


                this.Debug("[Draw]", "NotInProgress",
                        "[x,y] = " + x.ToString("0.00")
                    + ", " + y.ToString("0.00")
                    + "; pos = " + posX.ToString() + "x" + posY.ToString()
                    + "; SS = " + squareSize.ToString()
                    + "; phase = " + phase.ToString("0.0000"));                 
            }
            else
            {
                double startTime = (double)movementStartTime + movementStartTimePhase;
                double timePassed = ((double)time + phase - startTime);
                double progress = timePassed / (double)(this.Speed - movementStartTimePhase);
                double step = squareSize * progress;

                x = (this.lastPosX - 1) * squareSize + (posX - lastPosX) * step;
                y = (this.lastPosY - 1) * squareSize + (posY - lastPosY) * step;

                this.Debug("[Draw]", "InProgress",
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


            double scaleX = squareSize / image.ActualWidth;
            double scaleY = squareSize / image.ActualHeight;

            if (direction == MovementDirection.goLeft)
            {
                scaleX = (-1) * scaleX; // flip image
                
            }

            if (scale == null || (scale.ScaleX != scaleX || scale.ScaleY != scaleY))
            {
                scale = new ScaleTransform(scaleX, scaleY);
                image.RenderTransformOrigin = new Point(0, 0);
                image.RenderTransform = this.scale;
            }

            if (direction == MovementDirection.goLeft)
            {
                Canvas.SetLeft(image, x + squareSize);
                Canvas.SetTop(image, y);
            }
            else
            {
                Canvas.SetLeft(image, x);
                Canvas.SetTop(image, y);
            }

            Canvas.SetZIndex(image, 20); // 10 is for tile, 11 - 19 is for middle layer objects and GameObjects have 20 - 29 and 30 is for walls
        }

        public bool ProcessEvent(Int64 time, Event ev)
        {
            bool returnValue = false;

            switch (ev.what)
            {
                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    ProcessGoEvent(time, ev);

                    returnValue = true;
                    break;

                default:
                    returnValue = false;
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ev"></param>
        public void ProcessGoEvent(Int64 time, Event ev)
        {            
            this.MovementEventsInBuffer -= 1;

            MovementDirection md = EventType2MovementDirection(ev.what);

            // posX and posY are one-based!!!
            int nextFieldX = CoordinationOfMovementDirectionX(posX, md);
            int nextFieldY = CoordinationOfMovementDirectionY(posY, md);

            bool isThereWall = this.isThereWall(nextFieldX, nextFieldY);
            IGamePlugin gp = null;

            if (isThereWall == false)
            {
                gp = host.GetObstructionOnPosition(nextFieldX, nextFieldY);
            }

            // No wall and no other game object; just move my object
            if (isThereWall == false && gp == null)
            {
                PrepareMovement(ev.when, 0, (IGamePlugin)ev.who, ev.what);
            }
            // No wall and object can be moved
            else if (isThereWall == false && this.obstructionLevel > gp.ObstructionLevel)
            {
                int nextNextX = CoordinationOfMovementDirectionX(nextFieldX, md);
                int nextNextY = CoordinationOfMovementDirectionY(nextFieldY, md);

                if (this.isThereWall(nextNextX, nextNextY))
                {
                    host.MakeImmediatePlan("ProcessGoEvent", ev.who, EventType.hitToTheWall); // ev.who = this plugin
                }
                else
                {
                    IGamePlugin gp2 = host.GetObstructionOnPosition(nextNextX, nextNextY);

                    if (gp2 == null)
                    {
                        PrepareMovement(ev.when, 0, ev.who, ev.what);
                        PrepareMovement(ev.when, 0, gp, ev.what);
                    }
                    else
                    {
                        host.MakeImmediatePlan("ProcessGoEvent", ev.who, EventType.hitToTheWall); // ev.who = this plugin
                    }
                }
            }
            // Is there wall or object cannot be moved
            else
            {
                host.MakeImmediatePlan("ProcessGoEvent", ev.who, EventType.hitToTheWall); // ev.who = this plugin
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

        public static int CoordinationOfMovementDirectionX(int posX, MovementDirection movementDirection)
        {
            return posX + moves[(-1 + (int)movementDirection) * 2];
        }

        public static int CoordinationOfMovementDirectionY(int posY, MovementDirection movementDirection)
        {
            return posY + moves[(-1 + (int)movementDirection) * 2 + 1];
        }

        public static MovementDirection EventType2MovementDirection(EventType e)
        {
            if (e == EventType.goDown)
            {
                return MovementDirection.goDown;
            }
            else if (e == EventType.goUp) 
            {
                return MovementDirection.goUp;
            } 
            else if (e == EventType.goLeft) 
            {
                return MovementDirection.goLeft;
            }
            else if (e == EventType.goRight)
            {
                return MovementDirection.goRight;
            }
            else
            {
                throw new Exception("Unknonw movement direction: " + e.ToString());
            }
        }


        protected bool isThereWall(int x, int y)
        {
            if (x > fieldsX || x < 1 || y > fieldsY || y < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MakePlan(string debug, long time, IGamePlugin iGamePlugin, EventType eventType)
        {
            if (EventTypeLib.IsEventOfType(eventType, EventCategory.goXXX))
            {
                movementEventsInBuffer++;
            }

            host.MakePlan(debug, time, iGamePlugin, eventType);
        }

        public void MakeImmediatePlan(string debugMessage, IGamePlugin iGamePlugin, EventType eventType)
        {
            if (EventTypeLib.IsEventOfType(eventType, EventCategory.goXXX))
            {
                movementEventsInBuffer++;
            }
            
            host.MakeImmediatePlan(debugMessage, iGamePlugin, eventType);
        }
    }
}
