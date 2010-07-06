using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Interfaces
{
    public enum ErrorMessageSeverity
    {
        Low,
        Medium,
        High
    }

    public interface IErrorMessagesPresenter
    {
        void ErrorMessage(ErrorMessageSeverity ems, string originModule, string message);
    }
}
