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
    public class GetCategoryLeagues
    {
        private Leagues leagues = null;

        public Leagues Leagues
        {
            get { return leagues; }
        }


        public GetCategoryLeagues()
        {
        }

        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));
            xml.Load(xmlTextReader);

            try
            {
                XmlNodeList xmlLeagues = xml.SelectNodes("GetCategoryLeagues/Leagues/League");
                leagues = LeagueParsing.ParseLeagues(xmlLeagues);
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }
    }
}
