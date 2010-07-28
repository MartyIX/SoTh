using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Interfaces
{
    public interface IUserInquirer
    {
        void ShowMessage(string message); // Ok button displayed
        void ShowQuestion(string message, IEnumerable<string> answers, IUserInquiryAccepter accepter); // return one of given answers
        string PutQuestion(string message, IEnumerable<string> answers); // No event is fired
    }
}
