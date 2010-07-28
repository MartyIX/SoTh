using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Networking;
using Sokoban.Model;
using Sokoban.View.SetupNetwork;
using Sokoban.Interfaces;

namespace Sokoban
{
    public partial class MainWindow : IConnectionDialogPresenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">i.e., "/remote/GetInitLeagues/"</param>
        /// <returns>Response of the server</returns>
        private string getRequestOnServer(string request)
        {
            string output = ApplicationHttpReq.GetRequestOnServer(request, ProfileRepository.Instance, "MainProgram", consolePane);

            if (ApplicationHttpReq.LastError != String.Empty)
            {
                MessageBoxShow(ApplicationHttpReq.LastError);
                return String.Empty;
            }
            else
            {
                return output;
            }
        }


        #region IConnectionDialogPresenter Members

        private ConnectionDialog connectionDialog = null;

        public void Show(string ipAddress, int port, int leaguesID, int roundsID, IProfileRepository profileRepository, IErrorMessagesPresenter errorPresenter, IConnectionRelayer connectionRelayer, IGameMatch gameMatch)
        {
            if (connectionDialog != null)
            {
                connectionDialog.Close();
            }

            connectionDialog = new ConnectionDialog(ipAddress, port, leaguesID, roundsID,
                    ProfileRepository.Instance, consolePane, this, null);
            connectionDialog.Owner = this;
            connectionDialog.Closing += new System.ComponentModel.CancelEventHandler(modalWindow_Closing);
            connectionDialog.Show();
        }

        /// <summary>
        /// For solving http://stackoverflow.com/questions/2001046/wpf-how-to-correctly-implement-a-modal-dialog-on-top-a-non-modal-dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void modalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Activate();
        }

        void modalWindow_Closing()
        {
            this.Activate();
        }


        #endregion        


    }
}
