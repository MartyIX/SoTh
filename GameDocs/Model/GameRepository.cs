using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using Sokoban.Model.GameDesk;
using System.Threading;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model
{
    using Sokoban = Sokoban.Model.GameDesk.Sokoban;
    using System.Windows;
    using Sokoban.Lib.Events;      

    public partial class GameRepository : IBaseRepository, IGameRepository
    {
        public event TimeDelegate TimeReference;

        // Game fields
        public Calendar calendar;
        public Int64 time;
        public IsPermitted<EventType> IsPermitted;

        /// <summary>
        /// Table of size fieldsX x fieldsY and element of table is true if there is "Wall" on coordinate (x,y)   
        /// otherwise false.
        /// </summary>
        public bool[,] Walls;
        /// <summary>
        /// List of references to GameObjects "Aims"
        /// </summary>
        public List<GameObject> pAims;
        /// <summary>
        /// Arraylist of boxes
        /// </summary>
        public List<GameObject> pBoxes;
        /// <summary>
        /// Table of size fieldsX x fieldsY and element of table is null if there is not GameObject "Aim" on coordinate (x,y)   
        /// otherwise there is reference to the corresponding GameObject "Aim".
        /// </summary>        
        public GameObject[,] tableAims;
        public Sokoban pSokoban;


        // Objects on the gamedesk        
        private List<GameObject> gameObjects;
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
        private int stepsNo;

        

        public GameRepository()
        {
            dateTime = DateTime.Now.TimeOfDay;
            calendar = new Calendar();
            ud = null;
            IsPermitted = new IsPermitted<EventType>();

            // GameState
            gameState = GameState.NotLoaded;
            fieldsX = 10;
            fieldsY = 10;
            stepsNo = 0;
            time = 0;
        }

        public void Initialize()
        {

        }

        public void FireTimeReferenceEvent()
        {
            if (TimeReference != null) TimeReference(ref time);
        }


        /// <summary>
        /// Increment number of steps Sokoban did and show actual number in formular
        /// </summary>
        public void AddStep()
        {
            stepsNo++;
        }       

        /// <summary>
        /// Adds event to the calendar of model
        /// </summary>
        /// <param name="when">Time when action should be processed.</param>
        /// <param name="who"></param>
        /// <param name="what">What event should be processed.</param>
        public void MakePlan(string debug, Int64 when, GameObject who, EventType what)
        {
            if (EventTypeLib.IsEventOfType(what, EventCategory.goXXX))
            {
                who.MovementEventsInBuffer += 1;
                who.TimeMovementEnds = when + who.Speed;
            }
            
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
        public void MakeImmediatePlan(string debug, GameObject who, EventType what)
        {
            // TODO: explain why +1
            MakePlan("[Immediate]" + debug, time, who, what); 
        }

        public bool IsSomeObjectOnPosition(string description, int pozX, int pozY, ref GameObject box)
        {
            bool objectOnPosition = false;
            GameObject tempObject;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                tempObject = (GameObject)gameObjects[i];
                if (tempObject.Description == description && tempObject.posX == pozX && tempObject.posY == pozY)
                {
                    objectOnPosition = true;
                    box = tempObject;
                    break;
                }
            }
            return objectOnPosition;
        }

        public bool IsSomeObjectOnPosition(string description, int pozX, int pozY)
        {
            GameObject box = null;
            return IsSomeObjectOnPosition(description, pozX, pozY, ref box);
        }

        /// <summary>
        /// Function returns information if it is possible to come from direction @direction GameObject on position [pozX, pozY]
        /// </summary>
        /// <param name="GameObject">GameObject that wants to move on position [pozX, pozY]</param>
        /// <param name="pozX">x-coordinate on game desk</param>
        /// <param name="pozY">y-coordinate on game desk</param>
        /// <param name="direction">Direction we want to move GameObject</param>
        /// <returns>True if it is possible to move GameObject on position [pozX, pozY] otherwise false</returns>
        public bool IsObstructorOnPosition(GameObject GameObject, int pozX, int pozY, MovementDirection direction)
        {
            GameObject box = null;
            return IsObstructorOnPosition(GameObject, pozX, pozY, direction, ref box);
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
        public bool IsObstructorOnPosition(GameObject GameObject, int pozX, int pozY, MovementDirection direction, ref GameObject box)
        {
            box = null;

            bool boxOnPosition = IsSomeObjectOnPosition("B", pozX, pozY, ref box);

            // Move box 
            // =========
            if (GameObject == pSokoban && boxOnPosition == true
                  && pozX + moves[(-1 + (int)direction) * 2] <= fieldsX
                  && pozX + moves[(-1 + (int)direction) * 2] > 0
                  && pozY + moves[(-1 + (int)direction) * 2 + 1] <= fieldsY
                  && pozY + moves[(-1 + (int)direction) * 2 + 1] > 0)
            {

                // there's no GameObject on the position where we want the GameObject move to 
                if (IsSomeObjectOnPosition("M", pozX + moves[(-1 + (int)direction) * 2], pozY + moves[(-1 + (int)direction) * 2 + 1]) == false &&
                    IsSomeObjectOnPosition("B", pozX + moves[(-1 + (int)direction) * 2], pozY + moves[(-1 + (int)direction) * 2 + 1]) == false &&
                    Walls[pozX + moves[(-1 + (int)direction) * 2] - 1, pozY + moves[(-1 + (int)direction) * 2 + 1] - 1] == false
                   )
                {
                    return false;
                }
            }

            // Remaining cases
            // ================
            bool isObstructor = false;

            if (pozX > fieldsX || pozX < 1 || pozY < 1 || pozY > fieldsY || Walls[pozX - 1, pozY - 1] == true || boxOnPosition == true)
            {
                isObstructor = true;
            }

            return isObstructor;
        }

        public bool CheckIfIsEnd()
        {
            bool vsechnyNaMiste = true;

            for (int j = 0; j < pBoxes.Count; j++)
            {
                GameObject tmpBedna = (GameObject)pBoxes[j];

                if (tableAims[tmpBedna.posX - 1, tmpBedna.posY - 1] == null)
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

            MessageBox.Show("RoundFinished");
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
            MessageBox.Show("RoundRestart");
        }
    }
}
