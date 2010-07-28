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
        public void Reload()
        {
            Terminate();
            loadCurrentRound(game);
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
