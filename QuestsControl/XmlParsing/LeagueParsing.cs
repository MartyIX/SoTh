using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View;
using System.Xml;
using Sokoban.Model;

namespace Sokoban.Xml
{
    public static class LeagueParsing
    {
        public static Leagues ParseLeagues(XmlNodeList leaguesList)
        {
            Leagues l = new Leagues();

            if (leaguesList.Count > 0)
            {
                foreach (XmlNode league in leaguesList)
                {
                    int id = int.Parse(league["ID"].InnerText);
                    bool leagueLoaded = bool.Parse(league["Loaded"].InnerText);

                    Rounds rounds = RoundParsing.ParseRounds(league.SelectNodes("Rounds/Round"));
                    l.Add(new League(id, league["Name"].InnerText, leagueLoaded, rounds));
                }
            }

            return l;
        }
    }
}
