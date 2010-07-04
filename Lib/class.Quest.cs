using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Xml;
using System.IO;
using Sokoban;
using Sokoban.Lib.Xml;

namespace Sokoban.Model.GameDesk
{
    public class Quest : IQuest
    {
        private int actRound = 0;
        private XmlNodeList rounds;
        private string name;
        private string wholeQuestXml;

        /// <summary>
        /// The most common Quest
        /// </summary>
        /// <param name="questXML">XML that follows schema QuestSchema</param>
        public Quest(string questXml)
        {
            wholeQuestXml = questXml;
            XmlDocument xmlDoc = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(questXml));
            xmlDoc.Load(xmlTextReader);
            name = (xmlDoc.GetElementsByTagName("Name"))[0].InnerText;
            rounds = xmlDoc.GetElementsByTagName("Round");
        }

        #region IQuest Members

        public string Name
        {
            get { return name; }
        }

        public string ActualRoundXML
        {
            get { return rounds[actRound].OuterXml; }
        }

        public void MoveCurrentToNext()
        {
            if (IsLast())
            {
                throw new Exception("No more rounds in quest.");
            }
            else
            {
                actRound++;
            }
        }

        public bool IsLast()
        {
            if (rounds.Count == actRound + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string WholeQuestXml
        {
            get { return wholeQuestXml; }
        }

        #endregion
    }
}
