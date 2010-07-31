using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Networking;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl
    {
        void networkModule_DisconnectRequest(object obj)
        {
            if (networkModule != null)
            {
                networkModule.Terminate(true, NetworkMessageType.DisconnectRequest, obj);
            }

            Terminate(true); // preserveConnection = true because we terminated network connection above!

            applyShadeEffect();
        }

        public void Terminate()
        {
            Terminate(false);
        }

        public void Terminate(bool preserveNetworkConnection)
        {
            removeShadeEffect();            

            TerminateFirstPlayer();
            TerminateSecondPlayer(preserveNetworkConnection);
        }

        private void FirstPlayerIsFinishing()
        {
            visualSoundsContainer.Children.Clear();
            game.GameStatus = GameStatus.Finishing;
            this.PauseTime();
            this.applyShadeEffect();                        
        }

        private void TerminateFirstPlayer()
        {
            this.StopTime();
            visualSoundsContainer.Children.Clear();
            game.Terminate();
        }

        private void TerminateSecondPlayer(bool preserveConnection)
        {
            if (gameMode == GameMode.TwoPlayers)
            {
                if (networkModule != null && preserveConnection == false)
                {
                    networkModule.Terminate();
                }

                if (gameOpponent != null)
                {
                    gameOpponent.Terminate();
                }
            }
        }
    }
}
