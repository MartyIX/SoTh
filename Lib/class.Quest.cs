using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Xml;
using System.IO;
using Sokoban;
using Sokoban.Lib.Xml;
using Sokoban.Lib;

namespace Sokoban.Model.GameDesk
{
    public class Quest : IQuest
    {
        private int actRound = 0;
        private XmlNodeList rounds;
        private string name;
        private string wholeQuestXml;
        private OpeningMode openingMode;

        
        public Quest(string questXml) : this(OpeningMode.League, questXml)
        {

        }
        
        /// <summary>
        /// The most common Quest
        /// </summary>
        /// <param name="questXML">XML that follows schema QuestSchema</param>
        public Quest(OpeningMode openingMode, string questXml)
        {
            this.openingMode = openingMode;
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
            get {

                if (rounds.Count == 1 && openingMode == OpeningMode.Round)
                {
                    return rounds[0]["Name"].InnerText;
                }
                else 
                {
                    return name;
                }
            }
        }

        public int RoundsNo
        {
            get { return rounds.Count; }
        }

        public string ActualRoundXML
        {
            get { return rounds[actRound].OuterXml; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">One based</param>
        public void SetCurrentRound(int n)
        {
            if (rounds.Count < n)
            {
                throw new Exception("Quest contains only " + rounds.Count.ToString() +  " rounds.");
            }

            actRound = n - 1;
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
