using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game : IUserInquiryAccepter
    {       
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
            {
                throw new NotImplementedException("No handler for message: `" + message + "' (answer: " + answer + " )");
            }
        }
    }
}
