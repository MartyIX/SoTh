#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System;
#endregion

namespace Sokoban.View
{
    public partial class GameDesk : GraphicsDeviceControl
    {
        public void GameLogic()
        {
            /*
            if (form.IsGraphicsChangeEnabled && player.gameState != GameState.NotLoaded)
            {
                // for both players
                player.model.time += 1;
                //GameDeskView.Debug(player.model.time.ToString(), "Simulation");

                // first player
                if (form.playerTwo != player)
                {
                    player.model.Move();
                    //form.DEBUG_DrawFirstPlayer(player);
                }

                // second player
                if (form.playerTwo != null && player == form.playerTwo)
                {
                    if (form.playerTwo.gameState == GameState.BeforeFirstMove &&
                        form.playerTwo.model.calendar.CountOfEvents > 0)
                    {
                        form.playerTwo.gameState = GameState.Running;
                    }

                    if (form.playerTwo.gameState == GameState.Running)
                    {
                        form.DEBUG_DrawSecondPlayer(form.playerTwo);
                        form.playerTwo.model.Move();
                    }
                }

                if (player.gameState != GameState.NotLoaded)
                {
                    form.UpdateTimeAndSteps();
                }
            }*/
        }
    }
}