#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Xml;
using System.Xml.Schema;
#endregion

namespace Sokoban.Lib
{
    public delegate void RoundPropertiesDelegate(string gameVariant, string roundName, int fieldsX, int fieldsY);
    public delegate void GameObjectPropertiesDelegate(string pluginName, XmlNode node);


    /// <summary>
    /// This class load specified round from XML file (a league)
    /// </summary>
    public class XmlRoundReader
    {
        private string roundXml;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="form">Reference to main dialog</param>
        /// <param name="model">Reference to model of actual round</param>
        /// <param name="leagueDefinition">XML representation of a league</param>
        public XmlRoundReader(string roundXml)
        {
            this.roundXml = roundXml;
        }

        public event RoundPropertiesDelegate RoundPropertiesRead;
        public event GameObjectPropertiesDelegate GameObjectRead;

        /// <summary>
        /// Load given round from actually selected league.
        /// </summary>
        /// <param name="round">The number of round to load. Numbered from one.</param>        
        public void LoadRoundSettings()
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(roundXml));
            xml.Load(xmlTextReader);

            XmlNamespaceManager names = new XmlNamespaceManager(xml.NameTable);
            names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");


            XmlNode round = (xml.GetElementsByTagName("Round"))[0];

            // Round properties
            if (RoundPropertiesRead != null)
            {
                RoundPropertiesRead(
                    round["Variant"].InnerXml,
                    round["Name"].InnerXml,
                    int.Parse(round["Dimensions"]["Width"].InnerXml),
                    int.Parse(round["Dimensions"]["Height"].InnerXml)
                    );
            }

            // Adding objects to the desk
            int objectID = 0;
            XmlNodeList nodeList;

            nodeList = round.SelectNodes("./x:GameObjects/*", names);
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlNode node = nodeList[i];

                if (GameObjectRead != null)
                {
                    GameObjectRead(node.Name, node);
                }
            }
        }
    }
}