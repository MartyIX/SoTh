using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Model;
using Sokoban.Xml;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Xml
{
    public class GetLeagueRounds
    {
        private Rounds rounds;

        public Rounds Rounds
        {
            get { return rounds; }
        }


        public GetLeagueRounds()
        {
        }

        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));
            xml.Load(xmlTextReader);

            try
            {
                XmlNodeList xmlRounds = xml.SelectNodes("GetLeagueRounds/Rounds/Round");
                rounds = RoundParsing.ParseRounds(xmlRounds);
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }
    }
}
