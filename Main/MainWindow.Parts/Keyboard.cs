using System.Windows.Input;

namespace Sokoban
{
    public partial class MainWindow 
    {
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                if (gameManager != null && gameManager.ActiveGameControl != null)
                {
                    gameManager.ActiveGameControl.Reload();
                }
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


        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (gameManager != null && gameManager.ActiveGameControl != null)
            {
                gameManager.ActiveGameControl.KeyIsUp(sender, e);
            }
        }

    }
}
