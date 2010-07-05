﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model.Profile
{
    public class ProfileXmlServerResponse
    {
        private bool isLoggedIn = false;


        public ProfileXmlServerResponse()
        {
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
            }
            catch
            {
                isLoggedIn = false;
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
        }
    }
}