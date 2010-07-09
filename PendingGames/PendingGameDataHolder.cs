using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.View
{
    public class OfferItemData
    {
        public int ID { get; set; }
        public string RoundName { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }

        public OfferItemData(int id, string roundName, string username, string ipAddress)
        {
            this.ID = id;
            this.RoundName = roundName;
            this.Username = username;
            this.IPAddress = ipAddress;
        }
    }

}
