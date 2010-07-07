using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Networking
{
    public enum NetworkMessageType
    {
        ProgramVersion = 1,
        ListOfEvents = 2,
        Authentication = 3, // sends IP, Name, protocol version
        MessageDescriptor = 4,
        None = 5,
        DisconnectRequest = 6,
        DisconnectRequestConfirmation = 7
    }
}
