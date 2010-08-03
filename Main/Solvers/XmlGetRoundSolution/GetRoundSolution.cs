using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Solvers.XmlGetRoundSolution
{
    public class XmlGetRoundSolution
    {
        public bool Success { get; set; }
        public int RoundId { get; set; }
        public string Solution { get; set; }


        public XmlGetRoundSolution()
        {

        }


        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));

            try
            {
                xml.Load(xmlTextReader);

                XmlNode roundSolution = (xml.GetElementsByTagName("RoundSolution"))[0];
                Success = bool.Parse(roundSolution["Found"].InnerText);
                RoundId = int.Parse(roundSolution["ID"].InnerText);
                Solution = roundSolution["Solution"].InnerText;
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }
    }
}
