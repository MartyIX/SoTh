using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Networking
{
    [Serializable]
    public class DisconnectRequest
    {
        public DateTime DateTime { get; set; }

        public DisconnectRequest(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }

    [Serializable]
    public class DisconnectRequestConfirmation
    {
        public DateTime DateTime { get; set; }

        public DisconnectRequestConfirmation(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }

}
