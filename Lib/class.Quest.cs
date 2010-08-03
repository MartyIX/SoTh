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
using Sokoban.Interfaces;
using Sokoban.Model.Xml;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model.GameDesk
{
    public class Quest : IQuest
    {
        private int actRound = 0;
        private XmlNodeList rounds;
        private string name;
        private string wholeQuestXml;
        private OpeningMode openingMode;
        private IGameServerCommunication gameServerCommunication;

        
        public Quest(string questXml) : this(OpeningMode.League, questXml, null)
        {

        }
        
        /// <summary>
        /// The most common Quest
        /// </summary>
        /// <param name="questXML">XML that follows schema QuestSchema</param>
        public Quest(OpeningMode openingMode, string questXml, IGameServerCommunication gameServerCommunication)
        {
            this.gameServerCommunication = gameServerCommunication;
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

        public int CurrentRoundID
        {
            get { return int.Parse(rounds[actRound]["Id"].InnerText); }
        }

        public bool IsLeague
        {
            get { return openingMode == OpeningMode.League; }
        }

        public string CurrentRoundXML
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

        public bool StoreResult(TimeSpan time, string solution, out string message)
        {
            bool result = true;
            message = "";

            if (gameServerCommunication != null)
            {
                string s_time = time.Hours + ":" + time.Minutes + ":" + time.Seconds;
                string request = "/remote/RoundSolved/?rounds_id=" + CurrentRoundID +
                    "&time=" + s_time +
                    "&session_id=@session_id&hash=@hash&solution=" + solution;

                string xml = gameServerCommunication.GetRequestOnServer("ClassQuest",request, CurrentRoundID.ToString());

                if (xml != "error" && xml != "")
                {
                    XmlRoundSolved xmlRoundSolved = new XmlRoundSolved();

                    try
                    {
                        xmlRoundSolved.Parse(xml);
                    }
                    catch (InvalidStateException)
                    {
                        result = false;
                    }

                    if (result == true)
                    {
                        if (xmlRoundSolved.Success == false)
                        {
                            result = false;
                        }
                        else
                        {
                            message = xmlRoundSolved.Message;
                        }
                    }
                    
                }
                else
                {
                    result = false;
                }                
            }
            else
            {
                result = false;
            }

            return result;
        }

        #endregion
    }
}
