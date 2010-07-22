using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using Sokoban.Model.PluginInterface;
using System.ComponentModel;
using System.Windows;      

namespace Sokoban.Model
{
    public delegate void TimeDelegate(ref Int64 time);

    public partial class GameRepository : IBaseRepository, IGameRepository, IPluginParent, INotifyPropertyChanged
    {        
        // API
        public event TimeDelegate TimeReference;

        // Game fields
        public Calendar calendar;
        public Int64 time;
        public IsPermitted<EventType> IsPermitted;

        /// <summary>
        /// 
        /// </summary>
        public List<IGamePlugin> gameIndicators;
        /// <summary>
        /// 
        /// </summary>
        public List<IGamePlugin> movableElements;
        /// <summary>
        /// 
        /// </summary>        
        public IGamePlugin[,] fixedElements;
        /// <summary>
        /// 
        /// </summary>        
        public IGamePlugin[,] fixedTiles;
        /// <summary>
        /// 
        /// </summary>
        public List<IControllableByUserInput> controllableByUserObjects;

        
        public string RoundName
        {
            get { return roundName; }
            set { roundName = value; Notify("RoundName"); }
        }

        public string GameVariant
        {
            get;
            set;
        }

        public int StepsCount
        {
            get { return stepsCountGameObject.StepsCount; }
        }

        //
        // PRIVATE FIELDS
        //

        // Objects on the gamedesk        
        private List<IGamePlugin> gameObjects;
        private int[,] wave = null;
        private ArrayList waveQueue = null;
        private static int[] moves = { -1,  0, 
                                        1,  0, 
                                        0, -1, 
                                        0,  1 };
        // Repository fields
        private TimeSpan dateTime;
        private GameState gameState;
        private int fieldsX;
        private int fieldsY;

        private string roundName;
        private IControllableByUserInput stepsCountGameObject = null;
        private PluginService pluginService;
        

        public GameRepository()
        {
            Initialize();
        }

        public void Initialize()
        {
            dateTime = DateTime.Now.TimeOfDay;
            calendar = new Calendar();
            ud = null;
            IsPermitted = new IsPermitted<EventType>();

            // GameState
            gameState = GameState.NotLoaded;
            fieldsX = 10;
            fieldsY = 10;
            time = 0;

            pluginService = new PluginService(this);
        }
       
        /// <summary>
        /// Adds event to the calendar of model
        /// </summary>
        /// <param name="when">Time when action should be processed.</param>
        /// <param name="who"></param>
        /// <param name="what">What event should be processed.</param>
        public void MakePlan(string debug, Int64 when, IGamePlugin who, EventType what)
        {
            Event? ev = calendar.AddEvent(when, who, what);

            if (who.Description == "S")
            {
                DebuggerIX.WriteLine("[GR-MakePlan-AddedEv]", debug, ev.Value.ToString());
            }
        }

        /// <summary>
        /// Adds event to the calendar of model and the event is immediately processed because time is set to current time.
        /// </summary>
        /// <param name="who">GameObject</param>
        /// <param name="what">Event to process</param>
        public void MakeImmediatePlan(string debug, IGamePlugin who, EventType what)
        {
            // TODO: explain why +1
            MakePlan("[Immediate]" + debug, time, who, what); 
        }

        public bool IsSomeObjectOnPosition(string description, int posX, int posY, ref IGamePlugin box)
        {
            /*
            bool objectOnPosition = false;
            IGamePlugin tempObject;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                tempObject = (IGamePlugin)gameObjects[i];
                if (tempObject.Description == description && tempObject.PosX == posX && tempObject.PosY == posY)
                {
                    objectOnPosition = true;
                    box = tempObject;
                    break;
                }
            }
            return objectOnPosition;
            */

            return false;
        }

        public bool IsSomeObjectOnPosition(string description, int posX, int posY)
        {
            IGamePlugin box = null;
            return IsSomeObjectOnPosition(description, posX, posY, ref box);
        }

        /// <summary>
        /// Function returns information if it is possible to come from direction @direction GameObject on position [pozX, pozY]
        /// </summary>
        /// <param name="GameObject">GameObject that wants to move on position [pozX, pozY]</param>
        /// <param name="pozX">x-coordinate on game desk</param>
        /// <param name="pozY">y-coordinate on game desk</param>
        /// <param name="direction">Direction we want to move GameObject</param>
        /// <returns>True if it is possible to move GameObject on position [pozX, pozY] otherwise false</returns>
        public bool IsObstructorOnPosition(IGamePlugin gamePlugin, int pozX, int pozY, MovementDirection direction)
        {
            IGamePlugin box = null;
            return IsObstructorOnPosition(gamePlugin, pozX, pozY, direction, ref box);
        }

        /// <summary>
        /// Function returns information if it is possible to come from direction @direction GameObject on position [pozX, pozY]
        /// </summary>
        /// <param name="GameObject">GameObject that wants to move on position [pozX, pozY]</param>
        /// <param name="pozX">x-coordinate on game desk</param>
        /// <param name="pozY">y-coordinate on game desk</param>
        /// <param name="direction">Direction we want to move GameObject</param>
        /// <param name="box"></param>
        /// <returns>True if it is possible to move GameObject on position [pozX, pozY] otherwise false</returns>
        public bool IsObstructorOnPosition(IGamePlugin GameObject, int pozX, int pozY, MovementDirection direction, ref IGamePlugin box)
        {
            /*
            box = null;

            bool boxOnPosition = IsSomeObjectOnPosition("B", pozX, pozY, ref box);

            // Move box 
            // =========
            if (GameObject == controllableByUserObjects && boxOnPosition == true
                  && pozX + moves[(-1 + (int)direction) * 2] <= fieldsX
                  && pozX + moves[(-1 + (int)direction) * 2] > 0
                  && pozY + moves[(-1 + (int)direction) * 2 + 1] <= fieldsY
                  && pozY + moves[(-1 + (int)direction) * 2 + 1] > 0)
            {

                // there's no GameObject on the position where we want the GameObject move to 
                if (IsSomeObjectOnPosition("M", pozX + moves[(-1 + (int)direction) * 2], pozY + moves[(-1 + (int)direction) * 2 + 1]) == false &&
                    IsSomeObjectOnPosition("B", pozX + moves[(-1 + (int)direction) * 2], pozY + moves[(-1 + (int)direction) * 2 + 1]) == false &&
                    solidObstructionObjects[pozX + moves[(-1 + (int)direction) * 2] - 1, pozY + moves[(-1 + (int)direction) * 2 + 1] - 1] == false
                   )
                {
                    return false;
                }
            }

            // Remaining cases
            // ================
            bool isObstructor = false;

            if (pozX > fieldsX || pozX < 1 || pozY < 1 || pozY > fieldsY || solidObstructionObjects[pozX - 1, pozY - 1] == true || boxOnPosition == true)
            {
                isObstructor = true;
            }

            return isObstructor;
            */

            return false;
        }

        public bool CheckIfIsEnd()
        {
            /*
            bool vsechnyNaMiste = true;

            for (int j = 0; j < movableObstructionObjects.Count; j++)
            {
                GameObject tmpBedna = (GameObject)movableObstructionObjects[j];

                if (tableFunctionalTiles[tmpBedna.posX - 1, tmpBedna.posY - 1] == null)
                {
                    vsechnyNaMiste = false;
                    break;
                }
            }

            if (vsechnyNaMiste)
            {
                return true;
            }
            else
            {
                return false;
            }
             
            */
            return false;
        }

        public static int MoveDirCordX(int posX, MovementDirection movementDirection)
        {
            return posX + moves[(-1 + (int)movementDirection) * 2];
        }

        public static int MoveDirCordY(int posY, MovementDirection movementDirection)
        {
            return posY + moves[(-1 + (int)movementDirection) * 2 + 1];
        }


        /// <summary>
        /// The method is called when round is finished and is true one of these conditions:
        /// 1] Sokoban got all boxes at their places.
        /// 2] Sokoban was killed by a monster.
        /// </summary>
        public void RoundFinished(RoundEnd roundEnd)
        {
            /*
            if (form.gameType == GameType.SinglePlayer)
            {
                RoundFinished_SinglePlayer(roundEnd);
            }
            else if (form.gameType == GameType.TwoPlayers)
            {
                RoundFinished_TwoPlayers(roundEnd);
            }*/

            DebuggerIX.WriteLine("[RoundInfo]","RoundFinished");
        }

        public void RoundRestart()
        {
            /*
            if (form.gameType == GameType.SinglePlayer)
            {
                RoundRestart_SinglePlayer();
            }
            else if (form.gameType == GameType.TwoPlayers)
            {
                RoundRestart_TwoPlayers();
            }
            */
            DebuggerIX.WriteLine("[RoundInfo]", "RoundRestart");
        }


        public void Message(object message, IGamePlugin plugin)
        {

        }

        #region INotifyPropertyChanged Members
        
        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

    }
}
