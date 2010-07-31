using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;
using Sokoban.Model;
using Sokoban.Model.GameDesk;
using System.Windows.Controls;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl
    {
        public void RestartGame(GameDisplayType gameDisplayType)
        {
            bool executeRestart = true;
            removeShadeEffect();

            if (gameMode == GameMode.TwoPlayers && gameDisplayType == GameDisplayType.FirstPlayer && networkModule != null)
            {
                if (game.GameStatus == GameStatus.Finished)
                {
                    MessageBoxShow("You've already finished the game with opponent.");
                    executeRestart = false;
                }
                else
                {
                    networkModule.SendRestartMessage();
                }
            }

            if (executeRestart)
            {
                if (gameDisplayType == GameDisplayType.FirstPlayer)
                {
                    TerminateFirstPlayer(); // do not terminate opponent gamedesk
                    loadCurrentRound(game);
                }
                else if (gameDisplayType == GameDisplayType.SecondPlayer)
                {
                    TerminateSecondPlayer(true); // do not terminate opponent gamedesk
                    loadCurrentRound(gameOpponent);
                }
            }                                             
        }

        public void LoadNextRound()
        {
            if (!quest.IsLast())
            {
                quest.MoveCurrentToNext();
            }
            else
            {
                MessageBoxShow("There's no other round in this quest.");
            }

            Terminate();
            loadCurrentRound(game);
        }

        private void loadCurrentRound(Game game)
        {
            visualSoundsContainer.Children.Clear(); // remove all sounds

            if (game == this.game)
            {
                game.RegisterVisual(this.graphicsControl);
                game.PreRoundLoaded += new VoidChangeDelegate(game_PreRoundLoaded);
            }
            else
            {
                game.RegisterVisual(this.graphicsControlOpponent);
            }

            game.LoadCurrentRound();

            if (!game.IsQuestValid(this.quest))
            {
                DebuggerIX.WriteLine(DebuggerTag.Game, "Quest", "Quest is not valid!");
                throw new NotValidQuestException(game.QuestValidationErrorMessage);
            }

            if (game == this.game)
            {
                game.GameRepository.GameStarted += new VoidChangeDelegate(timeStart);
                DataContext = this;
                tbSteps.DataContext = this.Game.GameRepository;
                tbRoundName.DataContext = this.Game.GameRepository;
                Notify("GameRepository");
                game.StartRendering();
            }
            else
            {
                game.StartRendering();
            }
        }

        /// <summary>
        /// Called after the GameRepository instance is created but before round and plugins are loaded
        /// </summary>
        void game_PreRoundLoaded()
        {
            game.GameRepository.MediaElementAdded += new NewMediaElementDelegate(GameRepository_MediaElementAdded);
        }

        private void GameRepository_MediaElementAdded(MediaElement me)
        {
            if (!visualSoundsContainer.Children.Contains(me))
            {
                visualSoundsContainer.Children.Add(me);
            }
        }

    }

}
