using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl 
    {
        /// <summary>
        /// Capturing keys in order to move Sokoban
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameMode == GameMode.SinglePlayer || (gameMode == GameMode.TwoPlayers && networkModule.IsNetworkConnectionSetUp))
            {
                e.Handled = game.MoveRequest(e.Key);
            }
            else
            {
                MessageBoxShow("Connection has not been established yet. Please wait.");
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (gameMode == GameMode.SinglePlayer || (gameMode == GameMode.TwoPlayers && networkModule.IsNetworkConnectionSetUp))
            {
                e.Handled = game.StopMove(e.Key);
            }
        }
    }
}
