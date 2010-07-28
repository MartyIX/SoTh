using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Interfaces;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl : IUserInquiryAccepter
    {       
        public void UserInquiryResult(string message, string result)
        {
            throw new NotImplementedException();
        }        

        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }

    }
}
