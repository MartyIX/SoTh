#region usings
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
#endregion usings

namespace Sokoban
{
    public partial class Player
    {
		#region Methods (12) 

		// Public Methods (12) 

        /// <summary>
        /// Shows final list of players (if checked)
        /// Function is called from: "RoundFinished"
        /// </summary>
        public void finishedGame()
        {
            this.gameState = GameState.Finished;
            form.Message("Sokoban splnil úkol.");
            form.cbRecord.Enabled = true;

            DurationTime = DateTime.Now.Subtract(this.gameStart);

            if (form.cbSounds.Checked == true && this.ToString() == "PlayerOne")
            {
                gameDesk.pSokoban.PlayHappySound();
            }

            if (form.cbCharts.Checked == true)
            {
                //if (form.ChartsForm != null) form.ChartsForm.Close();

                //form.ChartsForm = new Dialogs.Charts(form, profile.User, form.actLeagueName,
                //                                            this.round, gameDesk.pSokoban.StepsNo,
                //                                           (int)DurationTime.TotalSeconds);
                //form.ChartsForm.Show();
            }
        }                    

        /// <summary>
        /// Prepares everything needed for start of given round
        /// </summary>
        public void InitializeRound()
        {
            DebugP("InitializeRound");
            form.KeyboardInitialize();
            //form.resetTimeAndSteps();

            profile.userCanPressArrows = false;

            gameDesk.Clear();                       
            gameDesk.logList.Clear();

            // creates model for this player
            model = new Simulation(form, this);

            if (this.ToString() == "PlayerTwo")
            {
                model.calendar.IsEnabledAddingEvents = false; // default for second player
            }

            //form.IsGraphicsChangeEnabled = false;
            //XmlRounds xmlRounds = gameDesk.LoadRoundFromXML(form.actLeagueXml, this.round);
            //form.IsGraphicsChangeEnabled = true;

            //this.roundsNo = xmlRounds.Count;

            this.gameState = GameState.BeforeFirstMove;

            gameDesk.HookRedrawing();
        }

        /// <summary>
        /// Start actions linked to the death of Sokoban
        /// </summary>
        public void KillSokoban()
        {
            if (form.cbSounds.Checked == true && this.ToString() == "PlayerOne")
            {
                gameDesk.pSokoban.PlayScreamSound();
            }

            gameDesk.pSokoban.StartKillingAnimation();
        }

        /// <summary>
        /// Stops game because Sokoban failed. Used when a monster killed Sokoban.
        /// </summary>
        public void LostGame()
        {
            DebugP("Game was lost.");
            form.Message("Sokoban failed his task.");
            form.cbRecord.Enabled = true;
            this.gameState = GameState.Lost;
            form.SetbStartCaption();
        }

        /// <summary>
        /// Starts and stops game pause.
        /// </summary>
        public void Pause()
        {            
            if (this.gameStart == DateTime.MinValue)
            {
                this.gameStart = DateTime.Now;
            }

            if (this.gameState == GameState.Paused || this.gameState == GameState.BeforeFirstMove)
            {
                DebugP("Game is running.");
                this.gameState = GameState.Running;
                form.cbRecord.Enabled = false;
                form.Message("Game is running.");
            }
            else if (this.gameState == GameState.Running)
            {
                DebugP("Game is paused.");
                this.gameState = GameState.Paused;
                form.Message("Pause");
            }

            form.SetbStartCaption();
        }

        /// <summary>
        /// The method is called when round is finished and is true one of these conditions:
        /// 1] Sokoban got all boxes at their places.
        /// 2] Sokoban was killed by a monster.
        /// </summary>
        public void RoundFinished(RoundEnd roundEnd)
        {
            if (form.gameType == GameType.SinglePlayer)
            {
                RoundFinished_SinglePlayer(roundEnd);
            }
            else if (form.gameType == GameType.TwoPlayers)
            {
                RoundFinished_TwoPlayers(roundEnd);
            }
        }

        public void RoundFinished_SinglePlayer(RoundEnd roundEnd)
        {
            DebugP("RoundFinished");
            gameDesk.logList.isLastFrameIsDeath = (roundEnd == RoundEnd.killedByMonster) ? true : false;

            if (roundEnd == RoundEnd.killedByMonster)
            {
                this.LostGame();
            }

            // Let the player to simulate the positions of objects
            if (form.cbRecord.Checked)
            {
                // we need to stop game after finishing task
                if (roundEnd == RoundEnd.taskFinished)
                {
                    finishedGame();
                }

                form.ShowLoggingWindow(true, true);
            }
            else
            {
                if (roundEnd == RoundEnd.taskFinished)
                {
                    this.model.calendar.IsEnabledAddingEvents = false;

                    gameDesk.RegisterScene(new Scene_EndOfRound(150, gameDesk), this.RunNextRound);
                }
            }
        }

        public void RoundFinished_TwoPlayers(RoundEnd roundEnd)
        {
            DebugP("RoundFinished");
            if (roundEnd == RoundEnd.killedByMonster)
            {
                this.LostGame();
                this.RunRound(this.round);

                if (this.ToString() == "PlayerOne") // first player provides his calendar
                {
                    form.playerTwo.FlushEventBuffer();
                    form.playerTwo.RegisterForeignCalendar(this);
                }
            }

            if (roundEnd == RoundEnd.taskFinished)
            {
                if (this.ToString() == "PlayerOne")
                {
                    form.playerTwo.SendVictoryMessage();
                    this.model.calendar.IsEnabledAddingEvents = false;
                    gameDesk.RegisterScene(new Scene_EndOfRound(150, gameDesk), this.StopGame);
                }
            }
        }

        public void RoundRestart()
        {
            if (form.gameType == GameType.SinglePlayer)
            {
                RoundRestart_SinglePlayer();
            }
            else if (form.gameType == GameType.TwoPlayers)
            {
                RoundRestart_TwoPlayers();
            }
        }

        /// <summary>
        /// Restarts actual round.
        /// </summary>
        public void RoundRestart_SinglePlayer()
        {
            DebugP("Round is being restarted.");
            this.RunRound(this.round);
            form.Message("Round was restarted.");
        }

        /// <summary>
        /// Restarts actual round.
        /// </summary>
        public void RoundRestart_TwoPlayers()
        {
            DebugP("Round is being restarted.");
            this.RunRound(this.round);

            if (this.ToString() == "PlayerOne")
            {
                Event ev = new Event(-7, model.calendar.LastEventTimeInCalendar + 1, 
                    form.playerTwo.gameDesk.pSokoban, EventType.restartRound, 0, 0);
                form.playerTwo.AddEventToBuffer(ev);
                form.playerTwo.FlushEventBuffer();
                form.playerTwo.RegisterForeignCalendar(this);
            }

            form.Message("Round was restarted.");
        }


        /// <summary>
        /// Starts round number @round of given league.
        /// </summary>
        /// <param name="round">Number of round to load. Numbered from one.</param>
        public void RunRound(int round)
        {
            this.round = round;
            this.gameStart = DateTime.MinValue;
            this.StopGame();
            this.InitializeRound();
            form.SetbStartCaption();

            form.Message(this.round.ToString() + ". round loaded. Press any key to start.");
        }

        /// <summary>
        /// The method is called in the moment when all boxes are the right place
        /// </summary>
        public void RunNextRound()
        {            
            this.finishedGame();
            this.round++;            

            if (roundsNo < round)
            {
                DebugP("[RunNextRound] League is won.");
                MessageBox.Show("Congratulation! You won all rounds of this league!\n Click OK to display first round of this league.", "Sokoban", MessageBoxButtons.OK);
                this.round = 1;
                RunRound(this.round);
            }
            else
            {
                DebugP("[RunNextRound] Round " + round.ToString() + " is being run.");
                profile.userCanPressArrows = false; // nechceme, aby uzivatel mel vyhodu a behem cekani plnil klavesovy buffer 
                RunRound(this.round);
                profile.userCanPressArrows = true;
            }
        }

        /// <summary>
        /// If a round is loaded then function stop this round - time is stopped and entire discrete simulation of game is stopped.
        /// </summary>
        public virtual void StopGame()
        {
            if (this.gameState != GameState.NotLoaded)
            {
                DebugP("Game is stopped.");
                form.cbRecord.Enabled = true;
                this.gameState = GameState.Stopped;
                form.SetbStartCaption();
            }
        }

        /// <summary>
        /// Get elapsed time from the start of the game
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetElapsedTime()
        {
            if (this.gameState == GameState.Running || this.gameState == GameState.Paused)
            {
                return DateTime.Now.Subtract(this.gameStart);
            }
            else
            {
                return DateTime.Now.Subtract(DateTime.Now); // returns zero time
            }
        }

		#endregion Methods 
    }
}