using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model
{
    using Sokoban = Sokoban.Model.GameDesk.Sokoban;
    using System.Threading;

    public partial class GameRepository : IBaseRepository
    {
        /// <summary>
        /// Processes given event
        /// </summary>
        /// <param name="ev">Event to process</param>
        public void ProcessEvent(Event? e)
        {
            Event ev = e.Value;

            switch (ev.what)
            {
                case EventType.guardRow:

                    this.GuardRow(ev);
                    break;

                case EventType.guardColumn:

                    this.GuardColumn(ev);
                    break;

                case EventType.pursuit:

                    #region pursuit

                    if (this.wave == null) wave = new int[fieldsX, fieldsY];
                    if (this.waveQueue == null) waveQueue = new ArrayList();

                    for (int j = 0; j < fieldsY; j++)
                    {
                        for (int i = 0; i < fieldsX; i++)
                        {
                            wave[i, j] = -1;
                        }
                    }

                    wave[ev.who.posX - 1, ev.who.posY - 1] = 0; // start
                    waveQueue.Clear();
                    waveQueue.Add(new Coordinate(ev.who.posX, ev.who.posY));

                    int g = 0;
                    int akt = 0;
                    int pSx = pSokoban.posX;
                    int pSy = pSokoban.posY;

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
                            int newX = tmp.x + moves[2 * t];
                            int newY = tmp.y + moves[2 * t + 1];

                            if (IsObstructorOnPosition(ev.who, newX, newY, (MovementDirection)(t + 1)) == false)
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

                                int newX = pSx + moves[2 * t];
                                int newY = pSy + moves[2 * t + 1];

                                if (newX - 1 >= 0 && newY - 1 >= 0 &&
                                    newX - 1 < fieldsX && newY - 1 < fieldsY &&
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
                        if (pSx - ev.who.posX < 0)
                        {
                            MakePlan("PursuitGLft", time + ev.who.Speed, ev.who, EventType.goLeft);
                        }
                        else if (pSx - ev.who.posX > 0)
                        {
                            MakePlan("PursuitGRgt", time + ev.who.Speed, ev.who, EventType.goRight);
                        }
                        else if (pSy - ev.who.posY < 0)
                        {
                            MakePlan("PursuitGUp", time + ev.who.Speed, ev.who, EventType.goUp);
                        }
                        else if (pSy - ev.who.posY > 0)
                        {
                            MakePlan("PursuitGDwn", time + ev.who.Speed, ev.who, EventType.goDown);
                        }
                    }

                    MakePlan("PursuitPrst", time + ev.who.Speed, ev.who, EventType.pursuit);

                    break;

                    #endregion pursuit

                case EventType.checkIfIsEnd:

                    #region checkIfIsEnd

                    CheckIfIsEnd();

                    #endregion checkIfIsEnd

                    break;

                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    #region goXXX

                    GameObject bedna = null;

                    // Object orientation
                    if (((MovementDirection)ev.what == MovementDirection.goLeft || (MovementDirection)ev.what == MovementDirection.goRight))
                    {
                        lock (ev.who)
                        {
                            ev.who.UI.Direction = (MovementDirection)ev.what;
                        }
                    }

                    if (IsObstructorOnPosition(ev.who, MoveDirCordX(ev.who.posX, (MovementDirection)ev.what),
                                                       MoveDirCordY(ev.who.posY, (MovementDirection)ev.what),
                                                       (MovementDirection)ev.what, ref bedna))
                    {
                        MakePlan("ETG-Obstr-Hit", time + 1, ev.who, EventType.hitToTheWall);
                    }
                    else
                    {
                        if (bedna != null)
                        {
                            Debugger.WriteLine("[BoxMovePrepare]", " Obj: " + ev.who.Description
                                + "; Raised from EventID: " + ev.EventID.ToString());
                            PrepareMovement(ev.when, bedna, ev); // move box
                            PrepareMovement(ev.when,ev.who, ev); // move Sokoban

                            break;
                        }
                        else
                        {
                            if (ev.who == pSokoban) AddStep();

                            Debugger.WriteLine("[PrepareMovement]", "Obj: " + ev.who.Description
                                + "; Raised from EventID: " + ev.EventID.ToString());
                            PrepareMovement(ev.when, ev.who, ev);
                        }

                        // Sokoban and a monster has met
                        if ((ev.who.Description == "M" && ev.who.posX == pSokoban.posX && ev.who.posY == pSokoban.posY) ||
                             ev.who.Description == "S" && IsSomeObjectOnPosition("M", ev.who.posX, ev.who.posY))
                        {
                            MakePlan("SokKilled", time + 1, ev.who, EventType.sokobanWasKilled);
                        }
                    }
                    break;

                    #endregion goXXX

                case EventType.wentRight:
                case EventType.wentLeft:
                case EventType.wentUp:
                case EventType.wentDown:

                    #region wentXXX

                    //if (time - ev.who.movementStartTime < ev.who.movementNoSteps)
                    if (ev.who.Description == "B" && CheckIfIsEnd())
                    {
                        RoundFinished(RoundEnd.taskFinished);
                    }

                    if (ev.who == pSokoban && pSokoban.heldKeyEvent != EventType.none 
                        && pSokoban.MovementEventsInBuffer == 0)
                    {
                        EventType newEvent = pSokoban.heldKeyEvent;
                        MakeImmediatePlan("SokRepMvmt", ev.who, newEvent);
                    }

                    break;

                    #endregion wentXXX

                case EventType.none:

                    break;

                case EventType.hitToTheWall:

                    #region hitToTheWall

                    /*
                    if (ev.who == pSokoban)
                    {
                        if (form.cbSounds.Checked == true && player.ToString() == "PlayerOne")
                        {
                            soundBank.PlayCue("Yawn");
                        }
                        form.Message("Sokoban narazil na překážku.");
                    }*/

                    // TODO: Move the code and create event

                    ev.who.TimeMovementEnds = time;


                    break;

                    #endregion hitToTheWall

                case EventType.endLostGame: // in this moment is set: calendar.IsEnabledAddingMovementEvents = false !!

                    #region endLostGame

                    if (pSokoban.IsDeathAnimationStarted)
                    {
                        MakePlan("SokDeathEnded", time + 5, ev.who, EventType.endLostGame);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        RoundFinished(RoundEnd.killedByMonster);
                    }
                    #endregion endLostGame

                    break;

                case EventType.setLeftOrientation:
                case EventType.setRightOrientation:

                    #region setOrientation

                    if (ev.who == pSokoban)
                    {
                        ((Sokoban)(ev.who)).SetOrientation(GameObject.Orientation2MovementDirection(ev.what));
                    }
                    break;

                    #endregion setOrientation

                case EventType.sokobanWasKilled:

                    #region sokobanWasKilled

                    //player.KillSokoban();
                    MakePlan("ETSokKilled", calendar.LastEventTimeInCalendar + 1, ev.who, EventType.endLostGame);
                    //player.calendar.IsEnabledAddingMovementEvents = false;

                    break;

                    #endregion sokobanWasKilled

                case EventType.restartRound:

                    RoundRestart();

                    break;
            }
        }

        void GuardColumn(Event ev)
        {
            // turn around
            if (IsObstructorOnPosition(ev.who, MoveDirCordX(ev.who.posX, ev.who.UI.Direction),
                                               MoveDirCordY(ev.who.posY, ev.who.UI.Direction), ev.who.UI.Direction))
            {
                ev.who.UI.Direction = (ev.who.UI.Direction == MovementDirection.goUp) 
                    ? MovementDirection.goDown : MovementDirection.goUp;
            }

            MakeImmediatePlan("GuardColETGo", ev.who, (EventType)ev.who.UI.Direction);
            MakePlan("GuardColETGC", time + ev.who.Speed, ev.who, EventType.guardColumn);
        }

        void GuardRow(Event ev)
        {
            // turn around
            if (IsObstructorOnPosition(ev.who, MoveDirCordX(ev.who.posX, ev.who.UI.Direction),
                                               MoveDirCordY(ev.who.posY, ev.who.UI.Direction), ev.who.UI.Direction))
            {
                ev.who.UI.Direction = (ev.who.UI.Direction == MovementDirection.goLeft) ? 
                    MovementDirection.goRight : MovementDirection.goLeft;
            }

            MakeImmediatePlan("GuardRowETGo", ev.who, (EventType)ev.who.UI.Direction);
            MakePlan("GuardRowETGR", time + ev.who.Speed, ev.who, EventType.guardRow);
        }
    }
}
