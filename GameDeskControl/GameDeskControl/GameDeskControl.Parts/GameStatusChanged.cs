using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl
    {
        /// <summary>
        /// May be called from first or second player!!
        /// </summary>
        /// <param name="gameChange"></param>
        public void GameChangedHandler(GameDisplayType gameDisplayType, GameChange gameChange)
        {
            if (gameMode == GameMode.SinglePlayer)
            {
                if (game.GameStatus == Lib.GameStatus.Running)
                {
                    if (gameChange == GameChange.Won)
                    {
                        if (quest.RoundsNo > 1)
                        {
                            if (!quest.IsLast())
                            {
                                ShowQuestion(inquirySinglePlayerPlayNextRoundOfLeague, new string[] { "Yes", "No" });
                            }
                            else
                            {
                                MessageBoxShow("Congratulations! You've won the round and completed the league!");
                            }
                        }
                        else
                        {
                            MessageBoxShow("Congratulations! You've won the round!");
                        }

                        FirstPlayerIsFinishing();
                    }
                    else if (gameChange == GameChange.StopCountingTime)
                    {
                        this.PauseTime();
                    }
                    else if (gameChange == GameChange.Lost)
                    {
                        FirstPlayerIsFinishing();
                        this.ShowQuestion(inquirySinglePlayerRestart, new string[] { "Yes", "No" }); // answer is processed in UserInquirer.cs                    
                    }
                }
            }
            else if (gameMode == GameMode.TwoPlayers)
            {
                if (gameDisplayType == GameDisplayType.FirstPlayer)
                {
                    if (game.GameStatus == Lib.GameStatus.Running)
                    {
                        if (gameChange == GameChange.Won)
                        {
                            MessageBoxShow("You have won the duel!");
                        }
                        else if (gameChange == GameChange.Lost)
                        {
                            MessageBoxShow("Your opponent have won the duel!");
                        }

                        FirstPlayerIsFinishing();
                    }
                }
                else
                {
                    if (gameOpponent.GameStatus == GameStatus.Running)
                    {
                        if (gameChange == GameChange.Restart)
                        {
                            this.RestartGame(GameDisplayType.SecondPlayer); // restart opponent game desk
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException("not implemented");
            }
        }

    }
}
