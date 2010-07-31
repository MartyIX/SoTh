using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;
using System.Windows.Controls;
using Sokoban.Model.GameDesk;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model
{
    public partial class GameRepository 
    {
        
        public int FieldsX
        {
            get { return fieldsX; }
        }

        public int FieldsY
        {
            get { return fieldsY; }
        }

        public int EventsInCalendar
        {
            get { return calendar.CountOfEvents; }
        }

        public IEnumerable<IGamePlugin> GetGameObjects
        {
            get { return gameObjects; }
        }

        public IEnumerable<IGamePlugin> AllPlugins
        {
            get { return gameObjects; }
        }

        public List<T> GetList<T>()
        {
            Type t = typeof(T);

            if (t is IControllableByUserInput)
            {
                return controllableByUserObjects as List<T>;
            }
            else if (t is IMovableElement)
            {
                return movableElements as List<T>;
            }
            else
            {
                throw new NotSupportedException("Type:" + t.ToString() + " is not supported");
            }
        }

        public IGamePlugin[,] GetFixedElements()
        {
            return fixedElements;            
        }

        public IGamePlugin[,] GetFixedTiles()
        {
            return fixedTiles;
        }

        public IGameVariant GameVariant
        {
            get { return this.gameVariant; }
        }

        public Int64 Time
        {
            get { return time; }
            set { time = value; }
        }

        public IGameRealTime GameRealTime
        {
            get { return this.game; }
            set { this.game = value; }
        }

        public PluginService PluginService
        {
            get { return pluginService; }
        }

        #region IPluginParent Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">one-based x-coord</param>
        /// <param name="y">one-based y-coord</param>
        /// <returns></returns>
        public IGamePlugin GetObstructionOnPosition(int x, int y)
        {
            IGamePlugin gp = null;

            // Fixed elements
                
            if (fixedElements[x - 1, y - 1] != null) 
            {
                gp = fixedElements[x - 1, y - 1];
            }
            else 
            {
                // Movable elements

                foreach (IMovableElement g in this.movableElements)
                {
                    if (g.PosX == x && g.PosY == y)
                    {
                        gp = g as IGamePlugin;
                        break;
                    }
                }
            }

            return gp;
        }

        void IPluginParent.PropertyChanged(string property)
        {
            Notify(property);
        }

        public void Terminate()
        {
            if (pluginService != null)
            {
                pluginService.Terminate();
            }

            pluginService = null;
            
            movableElements = null;
            fixedElements = null;
            controllableByUserObjects = null;
            gameObjects = null;
            DeskSizeChanged = null;
            GameObjectsLoaded = null;
        }

        /// <summary>
        /// Adds event to the calendar of model; called by plugins
        /// </summary>
        /// <param name="when">Time when action should be processed.</param>
        /// <param name="who"></param>
        /// <param name="what">What event should be processed.</param>
        /// <param name="force">Add the event even though the simulation is stopped</param>
        public void MakePlan(string debug, Int64 when, IGamePlugin who, EventType what, bool force)
        {
            if (gameDisplayType == GameDisplayType.FirstPlayer)
            {

                if (calendar.IsEnabledAddingEvents || force)
                {
                    // Send events to the opponent over network
                    if (gameMode == GameMode.TwoPlayers && gameDisplayType == GameDisplayType.FirstPlayer && networkService != null)
                    {
                        if (who != null)
                        {
                            networkService.SendNetworkEvent(who.ID, when, what, who.PosX, who.PosY);
                        }
                        else
                        {
                            networkService.SendNetworkEvent(-1, when, what, -1, -1);
                        }
                    }
                }

                Event? ev = calendar.AddEvent(when, who, what, force);
            }
        }


        /// <summary>
        /// Adds event to the calendar; for events from network
        /// </summary>
        /// <param name="ID">index to the gameObjects list</param>
        /// <param name="when"></param>
        /// <param name="what"></param>
        public void MakePlan(int ID, Int64 when, EventType what)
        {
            if (when == -1) // for restart event 
            {
                when = Math.Max(calendar.LastEventTimeInCalendar + 1, this.time);
            }
            
            if (ID == -1)
            {
                Event? ev = calendar.AddEvent(when, null, what);
            }
            else
            {
                if (gameObjects.Count - 1 < ID)
                    throw new InvalidDataFromNetworkException("ID " + ID + " of a game object is invalid.");

                IGamePlugin who = gameObjects[ID];
                Event? ev = calendar.AddEvent(when, who, what);
            }
        }

        public void ResumeSimulation()
        {
            calendar.IsEnabledAddingEvents = true;
        }

        public bool IsSimulationActive
        {
            get { return calendar.IsEnabledAddingEvents; }
        }


        public void StopSimulation()
        {
            calendar.IsEnabledAddingEvents = false;
        }

        /// <summary>
        /// Adds event to the calendar of model and the event is immediately processed because time is set to current time.
        /// </summary>
        /// <param name="who">GameObject</param>
        /// <param name="what">Event to process</param>
        public void MakeImmediatePlan(string debug, IGamePlugin who, EventType what)
        {
            MakePlan("[Immediate]" + debug, time, who, what, false);
        }

        public void Message(string messageType, object message, IGamePlugin plugin)
        {
            plugin.MessageReceived(messageType, message, null); // TODO
        }

        public event NewMediaElementDelegate MediaElementAdded;

        public void RegisterMediaElement(MediaElement me)
        {
            if (MediaElementAdded != null)
            {
                MediaElementAdded(me);
            }
        }

        #endregion
    }
}
