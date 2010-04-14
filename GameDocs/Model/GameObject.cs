using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using System.Windows.Media.Imaging;

namespace Sokoban.Model.GameDesk
{
    public partial class GameObject
    {
        public int ID;
        public int posX;
        public int posY;
        public int lastPosX;
        public int lastPosY;
        public string Description;
        public Int64 TimeMovementEnds = 0;        
        public int Speed;
        public int MovementEventsInBuffer = 0;

        public GameObjectUI UI;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ID">Unique number for GameObject</param>
        /// <param name="description">See "description" member of GameObject</param>
        /// <param name="pozX">See "pozX" member of GameObject</param>
        /// <param name="pozY">See "pozY" member of GameObject</param>
        /// <param name="direction">See "direction" member of GameObject</param>
        /// <param name="InitialEvent">First event of GameObject</param>
        /// <param name="speed">Speed of GameObject - typically for monsters</param>
        public GameObject(int ID, string description, int posX, int posY, MovementDirection direction, int speed)
        {
            this.ID = ID;
            this.Speed = speed;
            this.posX = posX;
            this.posY = posY;
            this.lastPosX = posX;
            this.lastPosY = posY;            
            this.Description = description;

            UI = new GameObjectUI();
            UI.Direction = direction;
            UI.LastMovementEvent = EventType.none;
            UI.MovementNumberOfFields = 0;

            UI.Image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            bi.UriSource = new Uri("pack://application:,,,/GameDocs;component/Resources/StaticObj/obj_" + this.Description + ".png");
            bi.EndInit();
            UI.Image.Source = bi;
        }

        public bool MovementInProgress(Int64 time)
        {
           return (time < UI.MovementEndTime) ? true : false;
        }

        public void SetOrientation(MovementDirection direct)
        {
            if (UI.Direction != direct && (direct == MovementDirection.goLeft || direct == MovementDirection.goRight))
            {
                UI.Direction = direct;
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

        public static MovementDirection Orientation2MovementDirection(EventType eventType)
        {
            if (eventType == EventType.setRightOrientation)
            {
                return MovementDirection.goRight;
            }
            else if (eventType == EventType.setLeftOrientation)
            {
                return MovementDirection.goLeft;
            }
            else
            {
                throw new Exception("Orientation2MovementDirection: Not valid eventType: " + eventType.ToString());
            }
        }
    }
}
