using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game : IUserInquiryAccepter
    {
        string inquiryRestart = "You've lost the round!\nDo you want to try again?";
        string inquirySinglePlayerPlayNextRoundOfLeague = "Congratulations! You've won the round!\n\nDo you want to play next round of the league?";

        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }

        private string PutQuestion(string message, IEnumerable<string> answers)
        {
            if (userInquirer != null)
            {
                return userInquirer.PutQuestion(message, answers);
            }
            else
            {
                return null;
            }
        }

        private void ShowQuestion(string message, IEnumerable<string> answers)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowQuestion(message, answers, this);
            }            
        }

        public void UserInquiryResult(string message, string answer)
        {
            if (message == inquiryRestart)
            {
                if (answer == "Yes")
                {
                    control.RestartGame();
                }
            }
            else if (message == inquirySinglePlayerPlayNextRoundOfLeague)
            {
                if (answer == "Yes")
                {
                    control.PlayNextRound();
                }
            }
            else
            {
                throw new NotImplementedException("No handler for message: `" + message + "' (answer: " + answer + " )");
            }
        }
    }
}
