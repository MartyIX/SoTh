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
    public delegate void RoundPropertiesDelegate(string roundName, int fieldsX, int fieldsY);
    public delegate void GameObjectPropertiesDelegate(int objectID, string description, int pozX, int pozY, MovementDirection direction, EventType InitialEvent, int speed);    


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
            XmlDocument xmlDoc = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(roundXml));
            xmlDoc.Load(xmlTextReader);

            XmlNode round = (xmlDoc.GetElementsByTagName("Round"))[0];

            // Round properties
            if (RoundPropertiesRead != null)
            {
                RoundPropertiesRead(
                    round["Name"].InnerXml,
                    int.Parse(round["Dimensions"]["width"].InnerXml),
                    int.Parse(round["Dimensions"]["height"].InnerXml)
                    );
            }

            // Adding objects to the desk
            int objectID = 0;
            XmlNodeList nodeList;

            // Aims, Boxes, Fakeboxes, Walls
            nodeList = round.SelectNodes("Aim | Box | FakeBox | Wall");
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlNode node = nodeList[i];

                if (GameObjectRead != null)
                {
                    GameObjectRead(objectID++, node.Name[0].ToString(),
                                     int.Parse(node["posX"].InnerXml), int.Parse(node["posY"].InnerXml),
                                     MovementDirection.none, EventType.none, 0 /* speed */);
                }
            }

            // Monsters
            nodeList = round.SelectNodes("Monster");
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlNode node = nodeList[i];

                if (GameObjectRead != null)
                {
                    GameObjectRead(objectID++, node.Name[0].ToString(),
                                     int.Parse(node["posX"].InnerXml), int.Parse(node["posY"].InnerXml),
                                     (MovementDirection)Enum.Parse(typeof(MovementDirection), "go" + node["orientation"].InnerXml),
                                     (EventType)Enum.Parse(typeof(EventType), node["firstState"].InnerXml),
                                     int.Parse(node["speed"].InnerXml));
                }
            }

            // Sokoban
            {
                XmlNode node = round["Sokoban"];

                if (GameObjectRead != null)
                {
                    GameObjectRead(objectID++, node.Name[0].ToString(),
                        int.Parse(node["posX"].InnerXml), int.Parse(node["posY"].InnerXml),
                        (MovementDirection)Enum.Parse(typeof(MovementDirection), "go" + node["twoWayOrientation"].InnerXml),
                        EventType.none,
                        0 /* speed */);
                }
            }           
        }
    }
}