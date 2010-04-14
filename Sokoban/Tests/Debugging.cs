
// === Directives for debugging Sokoban

#define twoPlayer
//#define onePlayer 

#if onePlayer
    #define DEBUG_enable_cal_1_to_file_logging
#endif

#if twoPlayer
    #define DEBUG_automatically_interconnect_apps
    #define DEBUG_open_second_player_window // show second gamedesk after the start of program
    #define DEBUG_enable_cal_1_to_file_logging
    #define DEBUG_enable_cal_2_to_file_logging
    //#define DEBUG_show_sokobans_positions
    //#define DEBUG_show_simulation_times
#endif

#define DEBUG_enable_keyboard_shortcuts

// === End of directives

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

namespace Sokoban
{
    public partial class GameDeskView : Form
    {
		#region Fields (1) 

        static StringBuilder debugInfo = new StringBuilder(10000);

		#endregion Fields 

		#region Methods (7) 

		// Public Methods (6) 

        public static void Debug(string message)
        {
            #if DEBUG            
            Debug(message, "general");
            #endif
        }

        public static void Debug(string message, string type)
        {
            #if DEBUG
            if (type == "Simulation") return;
            if (type == "Player2Net") return;
            if (type == "Player1Net") return;
            if (type == "Keyboard") return;
            if (type == "Calendar-adding") return;
            //if (type == "NetworkTimeSynchronization") return;
            //if (type == "Calendar") return;
            //if (type == "SecondPlayerEvents") return;            
            

            // Enabled for player 2:
            //if (type == "CalendarPlayerTwo") return;
            //if (type == "CalendarAddingToSecPlayer" && this.Text == "[1] Sokoban") return;            

            string logEntry = String.Format("{0:HH:mm:ss} ", DateTime.Now) + type.PadRight(20) + message.Trim(new char[] { '\n', ' ' }) + "\n";

            if (type == "NetworkTimeSynchronization" || type.ToLower() == "general" || type == "Calendar-adding")
            {
                System.Diagnostics.Debug.Write(logEntry);
            }
            else if (type == "CalendarPlayerOne")
            {
                #if DEBUG_enable_cal_1_to_file_logging
                tw1.Write(logEntry);
                #endif
            }
            else if (type == "CalendarPlayerTwo" || type == "CalAdd2thPlayer")
            {
                #if DEBUG_enable_cal_2_to_file_logging
                tw2.Write(logEntry);
                #endif
            }
            else
            {
                debugInfo.Append(logEntry);
            }
            #endif
        }

        public void DEBUG_DrawFirstPlayer(Player player)
        {
            #if DEBUG
            #if DEBUG_show_sokobans_positions
            lPozice1.Text = player.gameDesk.pSokoban.posX.ToString() + "x" + player.gameDesk.pSokoban.posY.ToString();
            #elif DEBUG_show_simulation_times
            lPozice1.Text = (player.model == null ) ? "NA" : player.model.Time.ToString();
            #endif
            #endif
        }

        public void DEBUG_DrawSecondPlayer(Player player)
        {
            #if DEBUG
            #if DEBUG_show_sokobans_positions
            lPozice2.Text = player.gameDesk.pSokoban.posX.ToString() + "x" + player.gameDesk.pSokoban.posY.ToString();
            #elif DEBUG_show_simulation_times
            lPozice2.Text = (player.model == null) ? "NA" : player.model.Time.ToString();
            #endif    
            #endif
        }

        public void DEBUG_KeysProcessing(Keys keyData)
        {
            #if DEBUG
            //System.Diagnostics.Debug.Write(keyData.ToString());

            #if DEBUG_enable_keyboard_shortcuts

            // clear RichTextBox with debug messages
            if (keyData == (Keys.Control | Keys.D3))
            {
                debugInfo.Length = 0;
                rbDebug.Text = "";
            }

            // show RichTextBox with debug messages
            if (keyData == (Keys.Control | Keys.D4))
            {
                rbDebug.Left = player.gameDesk.Left;
                rbDebug.Top = player.gameDesk.Top;
                rbDebug.Height = player.gameDesk.Height;
                rbDebug.Width = player.gameDesk.Width;
                rbDebug.Visible = (rbDebug.Visible == true) ? false : true;
                rbDebug.Text = (debugInfo == null) ? "" : debugInfo.ToString();
                IsGraphicsChangeEnabled = (IsGraphicsChangeEnabled == true) ? false : true;
            }

            // copy content of RichTextBox to the clipboard
            if (keyData == (Keys.Control | Keys.D5))
            {
                Clipboard.SetDataObject(debugInfo.ToString(), true);
            }

            if (keyData == (Keys.Control | Keys.D6))
            {
                tDelay.Visible = (tDelay.Visible) ? false : true;
            }

            if (keyData == (Keys.Control | Keys.D7))
            {
                Debug("Scene_EndOfRound");
                player.gameDesk.RegisterScene(new Scene_EndOfRound(800, player.gameDesk), player.RunNextRound);
            }

            if (keyData == (Keys.Control | Keys.D8))
            {
                Debug("Disconnect");
                playerTwo.GetConnection().Disconnect();
            }

            #endif

            #endif
        }

        public void DEBUG_OnLoad()
        {    
            #if DEBUG
            debugInfo.Length = 0;

            #if DEBUG_open_second_player_window
            bExtend_Click(null, null); // open second's player window on startup;

            #if (DEBUG_show_sokobans_positions || DEBUG_show_simulation_times)

                lPozice2.Left = playerTwo.gameDesk.Left;
                lPozice2.Top = playerTwo.gameDesk.Top + playerTwo.gameDesk.Height - lPozice2.Height;
                lPozice2.Visible = true;

            #endif

            #if DEBUG_automatically_interconnect_apps

                // automatically run the network connection
                if (CommonFunc.HowManyProcessesIsOpen("Sokoban") == 1)
                {
                    this.Text = "[1] Sokoban";
                    this.Left = 1;
                    this.Top = 1;
                    GameDeskView.Debug("[1] Sokoban is listenning", "general");
                    this.bListen_Click(null, null);
                }
                else
                {
                    this.Text = "[2] Sokoban";
                    this.Left = this.Width / 2 + 100;
                    this.Top = 1;
                    GameDeskView.Debug("[2] Sokoban tries to connect ...", "general");
                    this.bConnect_Click(null, null);
                }

                if (this.Text == "[1] Sokoban")
                {
                    #if DEBUG_enable_cal_1_to_file_logging
                    tw1 = new StreamWriter(@"D:\Prace\PowerShell\SokobanDebugging\1_PlayerOne.txt", false);
                    #endif
                    #if DEBUG_enable_cal_2_to_file_logging
                    tw2 = new StreamWriter(@"D:\Prace\PowerShell\SokobanDebugging\1_PlayerTwo.txt", false);                    
                    #endif
                }
                else
                {
                    #if DEBUG_enable_cal_1_to_file_logging
                    tw1 = new StreamWriter(@"D:\Prace\PowerShell\SokobanDebugging\2_PlayerOne.txt", false);
                    #endif
                    #if DEBUG_enable_cal_2_to_file_logging
                    tw2 = new StreamWriter(@"D:\Prace\PowerShell\SokobanDebugging\2_PlayerTwo.txt", false);
                    #endif
                }

            #endif
            #endif


            #if onePlayer
                #if DEBUG_enable_cal_1_to_file_logging
                    tw1 = new StreamWriter(@"D:\Prace\PowerShell\SokobanDebugging\1_PlayerOne.txt", false);
                #endif
            #endif


            #if (DEBUG_show_sokobans_positions || DEBUG_show_simulation_times)

                lPozice1.Left = player.gameDesk.Left;
                lPozice1.Top = player.gameDesk.Top + player.gameDesk.Height - lPozice1.Height;
                lPozice1.Visible = true;

            #endif            

            #endif
        }
		// Private Methods (1) 

        private void DEBUG_onClosed()
        {
            #if DEBUG
                #if DEBUG_enable_cal_1_to_file_logging
                tw1.Flush();
                tw1.Close();
                #endif
                #if DEBUG_enable_cal_2_to_file_logging
                tw2.Flush();            
                tw2.Close();
                #endif
            #endif
        }

		#endregion Methods 

        #if DEBUG_enable_cal_1_to_file_logging
        static TextWriter tw1;
        #endif
        #if DEBUG_enable_cal_2_to_file_logging
        static TextWriter tw2;
        #endif
    }
}