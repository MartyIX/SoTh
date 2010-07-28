using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model.Xml
{
    public class PendingGamesXmlServerResponse
    {
        public enum ActionType
        {
            Add,
            Delete
        }
        
        private bool success = false;
        private string errorMessage = null;
        private int offerID;

        public PendingGamesXmlServerResponse()
        {
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        public bool Success
        {
            get { return success; }
        }

        public int OfferID
        {
            get { return offerID; }
        }

        public void Parse(string response, ActionType at)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));

            try
            {
                xml.Load(xmlTextReader);

            //XmlNamespaceManager names = new XmlNamespaceManager(xml.NameTable);
            //names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");

                XmlNode login = (xml.GetElementsByTagName("GameOffer"))[0];

                if (at == ActionType.Add)
                {
                    success = (login["Added"].InnerText == "True");
                }
                else
                {
                    success = (login["Deleted"].InnerText == "True");
                }

                offerID = (login["ID"] != null) ? int.Parse(login["ID"].InnerText) : -1;

                errorMessage = login["Error"].InnerText;
            }
            catch
            {
                success = false;
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }


    }
}
