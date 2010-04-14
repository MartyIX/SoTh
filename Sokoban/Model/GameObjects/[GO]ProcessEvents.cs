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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
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
		#region Methods (2) 

		// Public Methods (2) 

        public void PrepareMovement(GameObject who, Event ev)
        {
            // This ensures that movement of Sokoban is "smoother"
            if (who.Description == "S")
            {
                ev.when = (who.model.Time > who.TimeMovementEnds) ? who.model.Time : who.TimeMovementEnds;
            }

            who.movementInProgress = true;
            who.movementNoSteps = who.Speed;
            who.movementStartTime = model.time;
            who.MakeMove((MovementDirection)ev.what);
            model.MakePlan(model.Time, who, (EventType)((int)(ev.what) + 10));
        }

        /// <summary>
        /// Processes given event
        /// </summary>
        /// <param name="ev">Event to process</param>
        public void ProcessEvent(Event ev)
        {
            switch (ev.what)
            {
                case EventType.guardRow:

                    this.GuardRow();
                    break;                    

                case EventType.guardColumn:

                    this.GuardColumn();
                    break;                    

                case EventType.pursuit:

                    #region pursuit

                    if (this.wave == null) wave = new int[player.gameDesk.maxX, player.gameDesk.maxY];
                    if (this.waveQueue == null) waveQueue = new ArrayList();

                    for (int j = 0; j < player.gameDesk.maxY; j++)
                    {
                        for (int i = 0; i < player.gameDesk.maxX; i++)
                        {
                            wave[i, j] = -1;
                        }
                    }

                    wave[this.posX - 1, this.posY - 1] = 0; // start
                    waveQueue.Clear();
                    waveQueue.Add(new Coordinate(this.posX, this.posY));

                    int g = 0;
                    int akt = 0;
                    int pSx = player.gameDesk.pSokoban.posX;
                    int pSy = player.gameDesk.pSokoban.posY;

                    while (g < waveQueue.Count)
                    {
                        Coordinate tmp = (Coordinate)waveQueue[g];
                        akt = wave[tmp.x - 1, tmp.y - 1];

                        if (tmp.x == pSx && tmp.y == pSy)
                        {
                            break;
                        }

                        for (int t = 0; t < 4; t++)
                        {
                            int newX = tmp.x + View.GameDesk.moves[2 * t];
                            int newY = tmp.y + View.GameDesk.moves[2 * t + 1];

                            if (gameDesk.IsObstructorOnPosition(this, newX, newY, (MovementDirection)(t + 1)) == false)
                            {
                                if (wave[newX - 1, newY - 1] == -1)
                                {
                                    wave[newX - 1, newY - 1] = akt + 1; // start
                                    waveQueue.Add(new Coordinate(newX, newY));
                                }
                            }
                        }
                        g++;
                    }

                    waveQueue.Clear();

                    // REKONSTRUKCE CESTY
                    // ================================================================================
                    akt = wave[pSx - 1, pSy - 1];

                    if (akt > -1) // akt == -1 znaci, ze neexistuje cesta k sokobanovi
                    {
                        Random rndNum = new Random();

                        while (wave[pSx - 1, pSy - 1] != 1)
                        {

                            if (wave[pSx - 1, pSy - 1] < 1)
                            {
                                break;
                            }

                            int[] tmpSmery = new int[] { 0, 1, 2, 3 };
                            int xTmpSmery = 3;

                            for (int u = 0; u < 4; u++)
                            {
                                int nahodne = rndNum.Next(0, xTmpSmery);
                                int t = tmpSmery[nahodne];
                                tmpSmery[nahodne] = tmpSmery[xTmpSmery];
                                xTmpSmery--;

                                int newX = pSx + View.GameDesk.moves[2 * t];
                                int newY = pSy + View.GameDesk.moves[2 * t + 1];

                                if (newX - 1 >= 0 && newY - 1 >= 0 &&
                                    newX - 1 < player.gameDesk.maxX && newY - 1 < player.gameDesk.maxY &&
                                    wave[newX - 1, newY - 1] < akt && wave[newX - 1, newY - 1] != -1)
                                {
                                    pSx = newX;
                                    pSy = newY;
                                    akt = wave[newX - 1, newY - 1];
                                    break;
                                }
                            }
                        }
                    }

                    if (akt == 1)
                    {
                        if (pSx - this.posX < 0)
                        {
                            model.MakePlan(this.model.Time + this.Speed, this, EventType.goLeft);
                        }
                        else if (pSx - this.posX > 0)
                        {
                            model.MakePlan(this.model.Time + this.Speed, this, EventType.goRight);
                        }
                        else if (pSy - this.posY < 0)
                        {
                            model.MakePlan(this.model.Time + this.Speed, this, EventType.goUp);
                        }
                        else if (pSy - this.posY > 0)
                        {
                            model.MakePlan(this.model.Time + this.Speed, this, EventType.goDown);
                        }
                    }

                    model.MakePlan(this.model.Time + this.Speed, this, EventType.pursuit);

                    break;

                    #endregion pursuit

                case EventType.checkIfIsEnd:

                    #region checkIfIsEnd

                    player.gameDesk.CheckIfIsEnd();

                    #endregion checkIfIsEnd

                    break;

                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    #region goXXX

                    GameObject bedna = null;

                    if (this == player.gameDesk.pSokoban && ((MovementDirection)ev.what == MovementDirection.goLeft || (MovementDirection)ev.what == MovementDirection.goRight))
                    {
                        model.MakePlan(this.model.Time, this, CommonFunc.Direction2Orientation(ev.what));
                    }

                    if (gameDesk.IsObstructorOnPosition(this, View.GameDesk.MoveDirCordX(posX, (MovementDirection)ev.what),
                                                              View.GameDesk.MoveDirCordY(posY, (MovementDirection)ev.what), 
                                                              (MovementDirection)ev.what, ref bedna))
                    {
                        model.MakePlan(this.model.Time + 1, this, EventType.hitToTheWall);
                    }
                    else
                    {
                        if (bedna != null)
                        {
                            this.PrepareMovement(bedna, ev); // move box
                            this.PrepareMovement(this, ev); // move Sokoban

                            break;
                        }
                        else
                        {
                            if (this == player.gameDesk.pSokoban) player.gameDesk.pSokoban.AddStep();

                            this.PrepareMovement(this, ev);
                        }

                        // Sokoban and a monster met
                        if ((this.Description == "M" && this.posX == gameDesk.pSokoban.posX && this.posY == player.gameDesk.pSokoban.posY) ||
                             this.Description == "S" && gameDesk.IsSomeObjectOnPosition("M", posX, posY))
                        {
                            model.MakePlan(model.time + 1, this, EventType.sokobanWasKilled);
                        }
                    }
                    break;

                    #endregion goXXX

                case EventType.movingRight:
                case EventType.movingLeft:
                case EventType.movingUp:
                case EventType.movingDown:

                    #region movingXXX

                    if (model.time - movementStartTime < movementNoSteps)
                    {
                        model.MakePlan(this.model.Time + 1, this, ev.what);
                    }
                    else
                    {
                        if (this.Description == "B" && player.gameDesk.CheckIfIsEnd())
                        {                            
                            player.RoundFinished(RoundEnd.taskFinished);
                        }

                        // synchronization of gameobject position
                        if (player.ToString() == "PlayerTwo")
                        {
                            this.posX = ev.posX;
                            this.posY = ev.posY;
                        }

                        if (this == player.gameDesk.pSokoban && form.isKeyDown == true)
                        {
                            EventType newEvent = form.repeatedSokobanEvent;
                            model.MakePlan(this.model.Time, this, newEvent);
                        }
                        else
                        {
                            movementInProgress = false;
                        }
                    }

                    break;

                    #endregion movingXXX

                case EventType.none:

                    break;

                case EventType.hitToTheWall:

                    #region hitToTheWall

                    if (this == player.gameDesk.pSokoban)
                    {
                        if (form.cbSounds.Checked == true && player.ToString() == "PlayerOne")
                        {
                            soundBank.PlayCue("Yawn");
                        }
                        //form.Message("Sokoban narazil na překážku.");
                    }
                    break;

                    #endregion hitToTheWall

                case EventType.endLostGame: // in this moment is set: calendar.IsEnabledAddingMovementEvents = false !!

                    #region endLostGame

                    if (player.gameDesk.pSokoban.IsDeathAnimationStarted)
                    {
                        model.MakePlan(model.Time + 5, this, EventType.endLostGame);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        player.RoundFinished(RoundEnd.killedByMonster);
                    }
                    #endregion endLostGame

                    break;

                case EventType.setLeftOrientation:
                case EventType.setRightOrientation:

                    #region setOrientation

                    Sokoban s = (Sokoban)ev.who;                        
                    s.ChangeOrientation(CommonFunc.Orientation2MovementDirection(ev.what));

                    break;

                    #endregion setOrientation

                case EventType.sokobanWasKilled:

                    #region sokobanWasKilled

                    player.KillSokoban();
                    model.MakePlan(model.calendar.LastEventTimeInCalendar + 1, this, EventType.endLostGame);
                    player.model.calendar.IsEnabledAddingMovementEvents = false;

                    break;

                    #endregion sokobanWasKilled

                case EventType.restartRound:

                    player.RoundRestart();

                    break;
            }
        }

		#endregion Methods 
    }
}