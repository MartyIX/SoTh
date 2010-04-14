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
using System.Diagnostics;
using System.Xml;
#endregion usings
/*
namespace Sokoban
{
    /// <summary>  
    /// Main form
    /// </summary>
    public partial class GameDeskView : Form
    {
		#region Fields (20) 
        /// <summary>
        /// Name of actual league
        /// </summary>
        public string actLeagueName = "";
         /// <summary>
        /// XML file with rounds definitions
        /// </summary>                                                       
        public string actLeagueXml = "";
        /// <summary>
        /// If program is run as executable file then program finds the path itself
        /// </summary>
        public string appPath = System.Windows.Forms.Application.StartupPath;
        /// <summary>
        /// Game server with Sokoban's web application; 
        /// (note that URI has to end with trailing backslash)
        /// </summary>
        private string baseURI = @"http://sokoban.martinvseticka.eu/";
        /// <summary>
        /// This path is relevant only if program is run from Visual Studio
        /// </summary>
        public string DEBUG_PATH = @"D:\Skola\Rocnikovy projekt - specifikace\Program\Sokoban_source\Sokoban";
        /// <summary>
        /// Type of game that is played on the form. Either single player of two players
        /// </summary>
        public GameType gameType;
         /// <summary>
        /// Charts form; not modal, but we don't want to display the window more than once
        /// </summary>
        public Form ChartsForm = null;
        /// <summary>
        /// Enables/disables refresh of XNA window
        /// </summary>
        public bool IsGraphicsChangeEnabled = true;
        /// <summary>
        /// Enable/disable initial displaying of login form
        /// </summary>
        public readonly bool IsLogInEnabled = false;
        /// <summary>
        /// Helper field for changing league (combobox) without any consequences
        /// </summary>
        private bool IsPermittedToChangeLeague = false;
        /// <summary>
        /// Variable is set to true always with change of league (cbLeague change); variable is set to false when loading a round (in model.cs)
        /// </summary>
        public bool LeagueChanged = true;
         /// <summary>
        /// Name of file where is list of leagues
        /// </summary>
        public readonly string LeaguesFileNameURI = @"listOfLeagues.xml";
        /// <summary>
        /// Main player of the game (always created)
        /// </summary>
        public Player player = null;
        /// <summary>
        /// Second player for two player game
        /// </summary>
        public SecondPlayer playerTwo = null;
        /// <summary>
        /// Says if replay-mode is On 
        /// </summary>
        public bool Replay = false;
        /// <summary>
        /// size of one square on game desk in pixels
        /// </summary>
        public int squareSize = 50;
        /// <summary>
        /// List of leagues that are available at server
        /// </summary>
        public XmlLeagues xmlLeagues;

        private bool isDisplayedLog = false;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Main dialog constructor
        /// </summary>
        public GameDeskView()
        {
            InitializeComponent();
        }

		#endregion Constructors 

		#region Properties (1) 

        public string BaseURI
        {
            get { return baseURI; }
            set { baseURI = value; }
        }

		#endregion Properties 

		#region Methods (19) 

		// Public Methods (5) 

        /// <summary>
        /// For sizes look on function: game.cs - vykresli_desku
        /// </summary>
        /// <param name="setFocusOnLast">If true sets focus on last item else on first one</param>
        /// <param name="showButtons">If true buttons "Continue" and "Save" are visible otherwise invisible</param>
        public void ShowLoggingWindow(bool setFocusOnLast, bool showButtons)
        {
            isDisplayedLog = true;

            AdjustForm();

            lbLog.Focus();
            lbLog.SelectedIndex = (setFocusOnLast) ? lbLog.Items.Count - 1 : 0;
        }

        /// <summary>
        /// Adjust the size of window according to player.gameDesk and player mode (single player, two players)
        /// </summary>
        public void AdjustForm()
        {
            this.SuspendLayout();
            
            int formWidth = 0; // width of form

            // settings under the (first player's) gameDesk

            player.gameDesk.Left = pMain.Left;
            cbLeague.Width = player.gameDesk.Width;

            int pSettingsTop = player.gameDesk.Top + player.gameDesk.Height + 5;            
            int besideGameDesk = player.gameDesk.Left + player.gameDesk.Width + 5;
            pSettings.Top = pSettingsTop;

            if (gameType == GameType.SinglePlayer)
            {
                if (playerTwo != null) playerTwo.gameDesk.Visible = false;

                if (isDisplayedLog)
                {
                    // right panel
                    pRight.Left = besideGameDesk;

                    // bConnect & bListen 
                    lbLog.Visible = true;
                    bConnect.Visible = false;
                    bListen.Visible = false;
                    bSave.Visible = true;
                    bContinue.Visible = true;

                    // bConnect & bListen & bContinue & bSave
                    bListen.Top = 0;

                    bContinue.Top = player.gameDesk.Top + player.gameDesk.Height - bContinue.Height;
                    bSave.Top = bContinue.Top - bSave.Height;

                    // log
                    CommonFunc.SetControlSize(lbLog,
                          new Point(0, player.gameDesk.Top - pRight.Top),
                          new Point(0, bSave.Top - 1),
                          new Point(pRight.Width, 0));
                }
                else
                {
                    // lbLog & bExtend
                    bExtend.Left = besideGameDesk;
                    lbLog.Visible = false;
                    bConnect.Visible = false;
                    bListen.Visible = false;
                    bSave.Visible = false;
                    bContinue.Visible = false;                    
                }

                bExtend.Top = player.gameDesk.Top + Convert.ToInt32(player.gameDesk.Height / 2) - Convert.ToInt32(bExtend.Height / 2);
                formWidth = bExtend.Left + bExtend.Width + 20;
            }
            else if (gameType == GameType.TwoPlayers)
            {
                // log
                pRight.Left = besideGameDesk;

                // playerTwo's gameDesk
                playerTwo.gameDesk.Left = pRight.Left + pRight.Width + 5;

                // bConnect & bListen & bContinue & bSave
                lbLog.Visible = true;
                bConnect.Visible = true;
                bListen.Visible = true;
                bSave.Visible = false;
                bContinue.Visible = false;
                playerTwo.gameDesk.Visible = true;

                // bConnect & bListen & bContinue & bSave
                bListen.Top = 0;
                bConnect.Top = bListen.Top + bListen.Height + 3;
                
                // log
                CommonFunc.SetControlSize(lbLog, 
                      new Point(0, player.gameDesk.Top - pRight.Top),
                      new Point(0, player.gameDesk.Top - pRight.Top + player.gameDesk.Height),
                      new Point(lbLog.Width, player.gameDesk.Top - pRight.Top));
                // bExtend
                bExtend.Left = playerTwo.gameDesk.Left + playerTwo.gameDesk.Width + 5;
                bExtend.Top = player.gameDesk.Top + Convert.ToInt32(player.gameDesk.Height / 2) - Convert.ToInt32(bExtend.Height / 2);

                formWidth = bExtend.Left + bExtend.Width + 20;
            }
            
            int formHeight = pSettingsTop + this.pSettings.Height + this.statusStrip1.Height + 40;
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Width = formWidth;
            this.Height = formHeight;
            this.MinimumSize = new Size(formWidth, formHeight);
            this.MaximumSize = new Size(formWidth, formHeight);            
            this.ResumeLayout();
        }

        // Class methods
        // ===========================================
        /// <summary>
        /// Method which is called during loading of GameDeskView 
        /// </summary>
        /// <param name="sender">Standard</param>
        /// <param name="e">Standard</param>
        public void GameDeskView_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.DoEvents(); // because of this.BringToFront() in constructor
            player = new Player(this, 23, 71);
            gameType = GameType.SinglePlayer;

            this.SetPathToSettings();
            this.LoadInitialLeague();
            player.InitializeRound();
            StL.Text = "Author: Martin Všetička, 2009";
            
            this.DEBUG_OnLoad();
        }

        /// <summary>
        /// Function resets values of time and steps on the main dialog.
        /// </summary>
        public void resetTimeAndSteps()
        {
            lTime.Text = "0:00";
            lSteps.Text = "0";
        }

        /// <summary>
        /// Set the caption of button bStart according to actual state of game.
        /// </summary>
        public void SetbStartCaption()
        {
            bStart.Text = (player.gameState == GameState.Running) ? "Stop" : "Start";
        }


		// Private Methods (14) 

        private void bConnect_Click(object sender, EventArgs e)
        {
            playerTwo.RegisterForeignCalendar(player);
            playerTwo.InitializeConnection(PlayerType.Client);
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            bContinue.Enabled = false;
            player.RunNextRound();
            bContinue.Enabled = true;
        }

        private void bExtend_Click(object sender, EventArgs e)
        {
            if (bExtend.Tag.ToString() == "Opened")
            {
                bExtend.Tag = "Closed";
                gameType = GameType.SinglePlayer;
            }
            else
            {                
                bExtend.Tag = "Opened";
                gameType = GameType.TwoPlayers;

                if (playerTwo == null)
                {
                    playerTwo = new SecondPlayer(this, lbLog.Left + lbLog.Width + 10, 71);
                    playerTwo.ConnectionMessages += new d_PlayerConnectionMessages(playerTwo_ConnectionMessages);
                    playerTwo.InitializeRound();
                }
            }
            
            AdjustForm();
        }

        private void bListen_Click(object sender, EventArgs e)
        {
            playerTwo.RegisterForeignCalendar(player);
            playerTwo.InitializeConnection(PlayerType.Server);
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                player.gameDesk.logList.LoadFromFile(openFileDialog1.FileName);
            }
        }

        private void bRestart_Click(object sender, EventArgs e)
        {
            player.RoundRestart();
        }

        private void bSave_Click(object sender, EventArgs e)
        {          
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                player.gameDesk.logList.SaveToFile(saveFileDialog1.FileName);            
            }
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            if (player.gameState == GameState.Lost || player.gameState == GameState.Stopped)
            {
                player.RoundRestart();
            }
            else
            {
                player.Pause();
            }
        }

        private void cbLiga_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsPermittedToChangeLeague)
            {
                DisableReplayMode();

                player.StopGame();
                this.LoadLeague(cbLeague.SelectedIndex + 1);
                player.InitializeRound();
            }
        }

        /// <summary>
        /// When user closes the main dialog
        /// </summary>
        private void GameDeskView_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (playerTwo != null)
            {
                playerTwo.Dispose();                
            }

            DEBUG_onClosed();
            player.StopGame();
        }

        private void GameDeskView_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            if (playerTwo != null) playerTwo.Dispose();
            player.StopGame();
        }

        private void GameDeskView_Paint(object sender, PaintEventArgs e)
        {
            if (player.profile.isUserAutenticated == false)
            {
                player.profile.isUserAutenticated = true;
                lUser.Text = player.profile.getUserName();
            }
        }

        /// <summary>
        /// Steps of game
        /// </summary>
        private void LBlog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (player.gameState == GameState.Finished || player.gameState == GameState.Lost)
            {
                this.SuspendLayout();

                string time;
                player.gameDesk.logList.MoveUpToEvent(player.gameDesk.logList.LastFrame, lbLog.SelectedIndex, out time);
                lTime.Text = time;
                player.gameDesk.logList.LastFrame = lbLog.SelectedIndex;
                lSteps.Text = lbLog.SelectedIndex.ToString();

                this.ResumeLayout();
            }
        }

        void playerTwo_ConnectionMessages(int messageID)
        {
            if (messageID == 1)
            {
                lStatus.Text = "Network status: Connection was lost";
            }
            else if (messageID == 2)
            {
                lStatus.Text = "Network status: Connection established";
            }
        }

		#endregion Methods 

        private void cbLeague_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        public void SetNetworkStatus(string message)
        {
            lStatus.Text = message;
        }

        private void SetTime(string text)
        {
            lTime.Text = text;
        }

        /// <summary>
        /// Function updates values of time and steps on the main dialog.
        /// </summary>
        public TimeSpan UpdateTimeAndSteps()
        {
            if (player.gameState == GameState.Running || player.gameState == GameState.Paused)
            {
                lSteps.Text = player.gameDesk.pSokoban.StepsNo.ToString();
                return DateTime.Now.Subtract(player.gameStart);
            }
            else
            {
                return DateTime.Now.Subtract(DateTime.Now); // returns zero time
            }
        }
    }
}
*/