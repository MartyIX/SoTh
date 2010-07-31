using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.Quests;
using Sokoban.Interfaces;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;
using Sokoban.Networking;
using Sokoban.Lib.Exceptions;
using Sokoban.View.SetupNetwork;
using Sokoban.Model;

namespace Sokoban
{
    public partial class MainWindow : IQuestHandler
    {
        public IGameMatch QuestSelected(int leaguesID, int roundsID, IQuest quest, GameMode gameMode)
        {
            return QuestSelected(leaguesID, roundsID, quest, gameMode, null); // IConnection == null
        }

        public IGameMatch QuestSelected(int leaguesID, int roundsID, IQuest quest, GameMode gameMode, IConnection connection)
        {
            bool wasOpened = true;
            IGameMatch gameMatch = null;
            string errorMesssage = "";

            //try
            //{
            
            gameMatch = this.gameManager.QuestSelected(leaguesID, roundsID, quest, gameMode);
            //}
            /*
            catch (PluginLoadFailedException e)
            {
                errorMesssage = "The league cannot be loaded. There's a problem in plugin: " + e.Message;
                wasOpened = false;
            }                        
            catch (NotValidQuestException e)
            {
                errorMesssage = "The league you've chosen cannot be run. More information in Console.";
                consolePane.ErrorMessage(ErrorMessageSeverity.Medium,
                    "GameManager", "The league you've chosen cannot be run. Additional message: " + e.Message);

                wasOpened = false;
            } 
            catch (Exception e) 
            {
                errorMesssage = "The league you've chosen cannot be run. More information in Console.";
                consolePane.ErrorMessage(ErrorMessageSeverity.Medium,
                    "GameManager", "The league you've chosen cannot be run. Additional message: " + e.Message);

                wasOpened = false;
            }*/

            if (wasOpened == false)
            {
                MessageBoxShow(errorMesssage);
            }

            if (wasOpened == true && gameMode == GameMode.TwoPlayers)
            {
                if (connection != null)
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
                else
                {
                    InitConnection ic = new InitConnection(leaguesID, roundsID, ProfileRepository.Instance, consolePane, this, gameMatch);
                    ic.Owner = this;
                    ic.Closing += new System.ComponentModel.CancelEventHandler(modalWindow_Closing);
                    ic.Show();
                }

                return gameMatch;
            }
            else
            {
                return null;
            }
        }

    }
}
