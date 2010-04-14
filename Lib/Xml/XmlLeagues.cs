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
using Sokoban.Lib.Http;
using Sokoban.Model;
#endregion

namespace Sokoban.Lib
{
    /// <summary>
    /// This class load specified league from XML file
    /// </summary>
    public class XmlLeagues
    {
        /// <summary>
        /// It sets "count" variable as side effect
        /// </summary>
        /// <returns>Names of leagues in XML file defined via constructor</returns>
        public static List<KeyValuePair<string, string>> GetLeaguesNames(string leaguesXML)
        {
            XmlDocument xmlDoc = new XmlDocument(); // create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(leaguesXML));

            xmlDoc.Load(xmlTextReader);
            XmlNodeList league = xmlDoc.GetElementsByTagName("League");

            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < league.Count; i++)
            {
                result.Add(new KeyValuePair<string, string>(league[i]["ID"].InnerXml, league[i]["Name"].InnerXml));
            }

            return result;
        }

        /// <returns>XML rounds names for given league</returns>
        public static List<KeyValuePair<string, string>> GetLeagueRoundsNames(string Xml)
        {                        
            XmlDocument xmlDoc = new XmlDocument();
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(Xml));
            
            xmlDoc.Load(xmlTextReader);
            XmlNodeList xmlRounds = xmlDoc.GetElementsByTagName("Round");

            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            foreach (XmlNode round in xmlRounds)
            {                
                result.Add(new KeyValuePair<string, string>(round["ID"].InnerXml, round["Name"].InnerXml));
            }

            return result;
        }		
    }
}