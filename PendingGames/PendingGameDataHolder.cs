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
        public int LeaguesID { get; set; }
        public int RoundsID { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }

        public OfferItemData(int id, string roundName, int leaguesID, int roundsID, string username, string ipAddress, int port)
        {
            this.ID = id;
            this.RoundsID = roundsID;
            this.LeaguesID = leaguesID;
            this.RoundName = roundName;
            this.Username = username;
            this.IPAddress = ipAddress;
            this.Port = port;
        }
    }

}
