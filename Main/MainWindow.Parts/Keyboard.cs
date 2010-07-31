using System.Windows.Input;
using Sokoban.Lib;

namespace Sokoban
{
    public partial class MainWindow 
    {
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                restartFirstPlayer();
            }
            else if (e.Key == Key.T)
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.DisplayBothDesks = !gameManager.ActiveGameControl.DisplayBothDesks;
                }
            }
            else
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.KeyIsDown(sender, e);
                }
            }
        }

        private void restartFirstPlayer()
        {
            if (gameManager != null && gameManager.ActiveGameControl != null)
            {
                gameManager.ActiveGameControl.RestartGame(GameDisplayType.FirstPlayer);
            }
        }


        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (gameManager != null && gameManager.ActiveGameControl != null)
            {
                gameManager.ActiveGameControl.KeyIsUp(sender, e);
            }
        }

    }
}
