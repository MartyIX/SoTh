#region using
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Sokoban.Model;
#endregion

namespace Sokoban
{
    using Color = Microsoft.Xna.Framework.Graphics.Color;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// Object on game desk
    /// </summary>
    public partial class GameObject
    {
        #region References fields

        protected View.GameDeskView form;
        /// <summary>
        /// Model to which the GameObject belongs
        /// </summary>
        public Simulation model;
        public View.GameDesk gameDesk;
        protected Player player;

        #endregion

        #region Game properties of object

        /// <summary>
        /// unique number for every GameObject
        /// </summary>
        public int objectID;

        /// <summary>
        /// x-coordinate of GameObject on game desk
        /// </summary>
        public int posX;

        /// <summary>
        /// y-coordinate of GameObject on game desk
        /// </summary>
        public int posY;
        private int speed;

        public int Speed
        {
            get
            {
                if (Description == "B" || Description == "F")
                {
                    return player.gameDesk.pSokoban.Speed;
                }

                if (Description == "S")
                {
                    return 15; // default speed of Sokoban
                }

                return speed;
            }
        }

        /// <summary>
        /// last X-coordinate
        /// </summary>
        public int lastPosX;
        /// <summary>
        /// last Y-coordinate
        /// </summary>
        public int lastPosY;

        protected int movementNoSteps = 0;
        protected long movementStartTime = 0;
        protected bool movementInProgress = false;
        public Int64 TimeMovementEnds { get { return movementStartTime + movementNoSteps + 1; } }

        public bool MovementInProgress
        {
            get { return movementInProgress; }
            set { movementInProgress = value; }
        }

        protected Stopwatch movementStopWatch;

        /// <summary>
        /// Position property with sideeffect of moving object on desk 
        /// </summary>
        public int PozY
        {
            get { return posY; }
            set { posY = value; }
        }

        /// <summary>
        /// Position property with sideeffect of moving object on desk 
        /// </summary>
        public int PozX
        {
            get { return posX; }
            set { posX = value; }
        }

        /// <summary>
        /// Direction to which the object is turned to
        /// </summary>
        public MovementDirection direction;

        /// <summary>
        /// "S" for Sokoban, "M" for monster, "A" for Aim, "F" for fakebox, ...
        /// </summary>
        public readonly string Description;

        protected int[,] wave = null;
        protected ArrayList waveQueue = null;


        #endregion

        #region XNA fields

        protected ContentManager content;
        protected SpriteBatch spriteBatch;
        protected ServiceContainer Services;
        protected GraphicsDevice GraphicsDevice;
        protected SoundBank soundBank;

        /// <summary>
        /// Texture of the game object (XNA)
        /// Initialized in this.InitializeGraphics
        /// </summary>
        protected Texture2D texture;
        #endregion

        #region constructor

        /// <summary>
        /// Just for unit tests !!
        /// </summary>
        public GameObject() : this(null, null, 1, "S", 1, 1, MovementDirection.none, EventType.none, 0)
        {           
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="form">Reference to the main form</param>
        /// <param name="model">Model to which the GameObject should belongs</param>
        /// <param name="objectID">Unique number for GameObject</param>
        /// <param name="description">See "description" member of GameObject</param>
        /// <param name="pozX">See "pozX" member of GameObject</param>
        /// <param name="pozY">See "pozY" member of GameObject</param>
        /// <param name="direction">See "direction" member of GameObject</param>
        /// <param name="InitialEvent">First event of GameObject</param>
        /// <param name="speed">Speed of GameObject - typically for monsters</param>
        public GameObject(View.GameDeskView form, Player player, int objectID, string description,
                          int posX, int posY, MovementDirection direction, EventType InitialEvent, int speed)
        {
            this.player = player;

            this.model = (player == null) ? null : player.model;
            this.gameDesk = (player == null) ? null : player.gameDesk;
            this.objectID = objectID;
            this.speed = speed;
            this.form = form;
            this.posX = posX;
            this.posY = posY;
            this.lastPosX = posX;
            this.lastPosY = posY;
            this.Description = description;

            movementStopWatch = null;

            if (this.model != null)
                inicialization(direction, InitialEvent);
        }
        #endregion

        #region GameObject initialization
        protected void inicialization(MovementDirection smer, EventType ITU)
        {
            if (Description == "S" && smer == MovementDirection.goLeft)
            {
                this.direction = MovementDirection.goRight;
            }
            else
            {
                this.direction = smer;
            }

            gameDesk.AddGraphicsComponent(this);
            model.MakePlan(0, this, ITU);
        }
        #endregion
    }
}