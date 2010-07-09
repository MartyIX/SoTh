using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;
using Sokoban.View;

namespace Sokoban.Model.Xml
{
    public class GetPendingGamesXmlServerResponse
    {
        private List<OfferItemData> gameList = new List<OfferItemData>();

        public List<OfferItemData> GameList
        {
            get { return gameList; }
            set { gameList = value; }
        }


        public GetPendingGamesXmlServerResponse()
        {
        }


        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));

            try
            {
                xml.Load(xmlTextReader);

                XmlNodeList pendingGames = xml.SelectNodes("PendingGames/Game");
                for (int i = 0; i < pendingGames.Count; i++)
                {
                    XmlNode game = pendingGames[i];

                    OfferItemData o = new OfferItemData(
                        int.Parse(game["OfferID"].InnerText),
                        game["RoundName"].InnerText,
                        game["OpponentName"].InnerText,
                        game["IP"].InnerText);

                    gameList.Add(o);
                }
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }


    }
}
