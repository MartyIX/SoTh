using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model.Profile
{
    public class ProfileXmlServerResponse
    {
        private bool isLoggedIn = false;
        private string sessionID = null;
        private string ip = null;


        public ProfileXmlServerResponse()
        {
        }

        public string SessionID
        {
            get { return sessionID; }
        }

        public string IP
        {
            get { return ip; }
        }


        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
        }

        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));
            xml.Load(xmlTextReader);

            //XmlNamespaceManager names = new XmlNamespaceManager(xml.NameTable);
            //names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");

            try
            {
                XmlNode login = (xml.GetElementsByTagName("Login"))[0];
                isLoggedIn = (login["Successful"].InnerText == "1");

                if (isLoggedIn == true)
                {
                    sessionID = login["SessionID"].InnerText;
                    ip = login["IP"].InnerText;
                }                
            }
            catch
            {
                isLoggedIn = false;
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }

        
    }
}
