using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;
using Sokoban;
using Sokoban.View;
using Sokoban.Lib;
using System.Windows;

namespace Sokoban
{
    public class UserInquirer : IUserInquirer
    {
        private Window window;
        private View.UserDialog lastDialog = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window">We need parent for our dialogs to be in front of app</param>
        public UserInquirer(Window window)
        {
            this.window = window;
        }

        public void ShowMessage(string message)
        {
            View.UserDialog d = new View.UserDialog(message, new string[] {"Ok"}, null);
            d.Completed += new VoidObjectStringStringDelegate(showMessage_Completed);
            d.Owner = window;
            lastDialog = d;
            d.Show();
        }

        public void AppendMessage(string message)
        {
            if (lastDialog != null)
            {
                lastDialog.AppendMessage(message);
            }
        }

        private void showMessage_Completed(object sender, string message, string answer)
        {
            lastDialog = null;

            window.Activate();

            if (sender != null && sender is IUserInquiryAccepter)
            {
                IUserInquiryAccepter acceptor = sender as IUserInquiryAccepter;
                acceptor.UserInquiryResult(message, answer);
            }
        }


        public void ShowQuestion(string message, IEnumerable<string> answers, IUserInquiryAccepter accepter)
        {
            View.UserDialog d = new View.UserDialog(message, answers, accepter);
            d.Completed += new VoidObjectStringStringDelegate(d_Completed);
            d.Owner = window;
            d.Show();
        }

        void d_Completed(object sender, string message, string answer)
        {
            window.Activate();
            
            if (sender != null && sender is IUserInquiryAccepter)
            {
                IUserInquiryAccepter acceptor = sender as IUserInquiryAccepter;
                acceptor.UserInquiryResult(message, answer);
            }
        }

        string putQuestionAnswer = null;

        public string PutQuestion(string message, IEnumerable<string> answers)
        {
            View.UserDialog d = new View.UserDialog(message, answers, null);
            d.Owner = window;
            d.Completed += new VoidObjectStringStringDelegate(putQuestion_Completed);
            d.ShowDialog();
            
            // here is callback putQuestion_Completed called

            return putQuestionAnswer;
        }

        private void putQuestion_Completed(object sender, string message, string answer)
        {
            putQuestionAnswer = answer;
        }
    }
}
