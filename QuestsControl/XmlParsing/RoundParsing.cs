using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View;
using System.Xml;
using Sokoban.Model;

namespace Sokoban.Xml
{
    public static class RoundParsing
    {
        public static Rounds ParseRounds(XmlNodeList roundsNodeList)
        {
            Rounds rounds = new Rounds();

            if (roundsNodeList.Count > 0)
            {
                foreach (XmlNode round in roundsNodeList)
                {
                    int roundID = int.Parse(round["ID"].InnerText);

                    TimeSpan bestTime;

                    if (!TimeSpan.TryParse(round["BestTime"].InnerText, out bestTime))
                    {
                        bestTime = TimeSpan.Zero;
                    }

                    rounds.Add(new Round(roundID, round["Name"].InnerText, round["Variant"].InnerText, bestTime));
                }                
            }

            return rounds;                            
        }
    }
}
