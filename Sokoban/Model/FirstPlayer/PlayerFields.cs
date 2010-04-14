using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sokoban;
using Sokoban.View;

namespace Sokoban
{
    public partial class Player
    {
		#region Fields (11) 

        protected GameDeskView form;
        public GameDesk gameDesk;
        /// <summary>
        /// Time when user finished round (counter doesn't stop on pauses)
        /// </summary>
        public DateTime gameEnd = DateTime.MaxValue;
        /// <summary>
        /// Time when user started to play
        /// </summary>
        public DateTime gameStart = DateTime.MinValue;
        /// <summary>
        /// State in which game is right now
        /// </summary>
        public GameState gameState = GameState.NotLoaded;
        /// <summary>
        /// Discrete simulation model
        /// </summary>
        public Simulation model = null;
        /// <summary>
        /// User profile
        /// </summary>
        public ProfileRepository profile = null;
        /// <summary>
        /// actual round in league
        /// </summary>
        public int round = 1;
        /// <summary>
        /// How many rounds the actual league has
        /// </summary>
        public int roundsNo = 0;
        /// <summary>
        /// Time in which user finished the round
        /// </summary>
        public TimeSpan DurationTime;

		#endregion Fields 

		#region Constructors (1) 

        public Player(GameDeskView form, int posX, int posY)
        {
            this.form = form;

            round = 1;
            roundsNo = 0;
            /*
            gameDesk = new GameDesk(form, this);
            gameDesk.Location = new System.Drawing.Point(posX, posY);
            gameDesk.Name = "gameDesk";
            gameDesk.Size = new System.Drawing.Size(396, 386); // not needed
            gameDesk.TabIndex = 0;
            gameDesk.Text = "gameDesk";
            form.Controls.Add(gameDesk);
            */
            //profile = new ProfileRepository(form, this);
            profile = ProfileRepository.Instance;
        }

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        /// <summary>
        /// Debug player's state
        /// </summary>
        /// <param name="message"></param>
        public void DebugP(string message)
        {
            GameDeskView.Debug(this.ToString() + " " + message, "general");
        }

        public override string ToString()
        {
            return "PlayerOne";
        }


		#endregion Methods 
    }
}
