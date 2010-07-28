using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban.Networking;
using Sokoban.Lib.Exceptions;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;

namespace Sokoban
{
    public partial class MainWindow : IConnectionRelayer
    {
        public void Connect(IConnection connection, IGameMatch gameMatch, Authentication autentization, int leaguesID, int roundsID)
        {
            if (gameMatch != null) // gameMatch is returned from "server"
            {
                try
                {
                    gameMatch.SetNetworkConnection(connection);
                }
                catch (InvalidStateException e)
                {
                    MessageBoxShow("Error in setting up network game occured: " + e.Message);
                }
            }
            else // "client" doesn't have GameDeskControl opened by default
            {
                string questXml = this.getRequestOnServer("/remote/GetLeague/" + leaguesID.ToString());

                if (questXml != "error" && questXml != "")
                {
                    Quest q = new Quest(questXml);
                    IGameMatch gm = this.QuestSelected(leaguesID, roundsID, q, GameMode.TwoPlayers, connection);
                }
                else
                {
                    consolePane.ErrorMessage(ErrorMessageSeverity.Medium, "MainProgram", "Server returned empty response. The problem is propably at server-side.");
                    MessageBoxShow("The league cannot be opened. A problem is probably on the side of server.");
                    connection.CloseConnection();
                }
            }
        }
    }
}
