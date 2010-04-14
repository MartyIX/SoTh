using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Xml;

namespace Sokoban
{
    /// <summary>
    /// Helper class for storing Cartesian coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// x-coordinate
        /// </summary>
        public int x;
        /// <summary>
        /// y-coordinate
        /// </summary>
        public int y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// Main form
    /// </summary>
    public partial class GameDeskView : Form
    {
        /// <summary>
        /// Function that wraps inserting messages to status bar
        /// </summary>
        /// <param name="text">Text that will appear in status bar</param>
        public void Message(string text)
        {
            StL.Text = DateTime.Now.ToString("HH:mm:ss") + " " + text;
        }

        /// <summary>
        /// Replay mode is enabled when loading game via "Load" button. This function enables buttons "start/pause", "Again"
        /// and deletes "-- Replay --" from the list of leagues where it is to warn user he/she is in replay mode.
        /// </summary>
        public void DisableReplayMode()
        {
            if (Replay == true)
            {
                cbLeague.Items.RemoveAt(cbLeague.Items.Count - 1);
                Replay = false;

                bStart.Enabled = true;
                bRestart.Enabled = true;
            }
        }

        /// <summary>
        /// Replay mode is enabled when loading game via "Load" button. This function disables buttons "start/pause", "Again"
        /// and adds "-- Replay --" to the list of leagues where it is to warn user he/she is in replay mode.
        /// </summary>
        public void EnableReplayMode()
        {
            if (Replay == false)
            {
                cbLeague.Items.Add("-- Replay --");
                ChangeLeagueWithoutSideEffects(cbLeague.Items.Count - 1);
                Replay = true;

                bStart.Enabled = false;
                bRestart.Enabled = false;
            }
        }

        /// <summary>
        /// Function solves problems with path to the program when debugging from Visual Studio.
        /// </summary>
        private void SetPathToSettings()
        {
            #if (DEBUG)                
                    this.appPath = this.DEBUG_PATH; // for debugging purposes
                    this.Message("Run from Visual Studio: " + this.appPath);
            #endif
        }

        /// <summary>
        /// Normally, changing league would start first round of the newly selected league. When you don't want that you can use this function.
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void ChangeLeagueWithoutSideEffects(int selectedIndex)
        {
            bool lastState = IsPermittedToChangeLeague;
            IsPermittedToChangeLeague = false;
            cbLeague.SelectedIndex = selectedIndex;
            IsPermittedToChangeLeague = lastState;
        }

        public EventType Keys2EventType(Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                return EventType.goLeft;
            }
            else if (keyData == Keys.Right)
            {
                return EventType.goRight;
            }
            else if (keyData == Keys.Up)
            {
                return EventType.goUp;
            }
            else if (keyData == Keys.Down)
            {
                return EventType.goDown;
            }
            else
            {
                throw new Exception("Keys2EventType: Not supported key.");
            }
        }
    }
}