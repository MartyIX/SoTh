using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Interfaces
{
    public interface IUserInquiryAccepter
    {
        void UserInquiryResult(string message, string result);
    }
}
