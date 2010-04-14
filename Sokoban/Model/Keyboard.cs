using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

namespace Sokoban
{
    using ThreadState = System.Threading.ThreadState;

    public partial class GameDeskView : Form
    {
		#region Fields (2) 

        public bool isKeyDown;
        public EventType repeatedSokobanEvent;

		#endregion Fields 

		#region Methods (6) 

		// Public Methods (1) 

        /// <summary>
        /// Necesarry to call during initialization of round
        /// </summary>
        public void KeyboardInitialize()
        {
            isKeyDown = false;
            repeatedSokobanEvent = EventType.none;
            SetPropertyToAllControls(this.Controls);
        }
		
        // Protected Methods (1) 

        protected void KeyboardProcessing(Keys keyData)
        {
            if (player.profile.userCanPressArrows && (player.gameState == GameState.Paused || player.gameState == GameState.BeforeFirstMove))
            {
                player.Pause();
            }

            if (player.profile.userCanPressArrows && player.gameState == GameState.Running && isKeyDown == false)
            {
                if (keyData == Keys.Down || keyData == Keys.Up || keyData == Keys.Left || keyData == Keys.Right)
                {
                    Int64 whenMovementIsPossible = Math.Max(player.gameDesk.pSokoban.model.Time, player.gameDesk.pSokoban.TimeMovementEnds);
                    Debug("(key pressed " + keyData.ToString() + " at time " + player.gameDesk.pSokoban.model.Time.ToString() + ")", "Keyboard");
                    repeatedSokobanEvent = Keys2EventType(keyData);

                    if (player.gameDesk.pSokoban.MovementInProgress == false)
                    {
                        player.gameDesk.pSokoban.model.MakePlan(player.model.time, player.gameDesk.pSokoban, repeatedSokobanEvent);
                        player.gameDesk.pSokoban.MovementInProgress = true;
                    }
                }

                DEBUG_KeysProcessing(keyData);
            }
        }
		// Private Methods (4) 

        /// <summary>
        /// Disables navigation on form via arrows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        private void GameDeskView_KeyDown(object sender, KeyEventArgs e)
        {
            Debug("KeyDown " + e.KeyData.ToString(), "Keyboard");
            KeyboardProcessing(e.KeyData);
            isKeyDown = true;
        }

        private void GameDeskView_KeyUp(object sender, KeyEventArgs e)
        {
            Debug("KeyUp", "Keyboard");
            isKeyDown = false;
        }

        /// <summary>
        /// Set event PreviewKeyDown for controls in order to stop navigation on form via arrows
        /// </summary>
        /// <param name="cc"></param>
        private void SetPropertyToAllControls(Control.ControlCollection cc)
        {
            if (cc != null)
            {
                foreach (Control control in cc)
                {
                    control.PreviewKeyDown += new PreviewKeyDownEventHandler(control_PreviewKeyDown);
                    SetPropertyToAllControls(control.Controls);
                }
            }
        }

		#endregion Methods 

        /// <summary>
        /// We override ProcessCmdKey for the case we want to regulate keys
        /// </summary>
        /*
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Debug(this.ActiveControl.ToString());            

            return base.ProcessCmdKey(ref msg, keyData);
        }*/
    }
}