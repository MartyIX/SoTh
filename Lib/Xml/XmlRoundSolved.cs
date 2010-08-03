using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;
using Sokoban.View;

namespace Sokoban.Model.Xml
{
    public class XmlRoundSolved
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public int RoundId { get; set; }
        public string Message { get; set; }
        
        
        public XmlRoundSolved()
        {

        }


        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));

            try
            {
                xml.Load(xmlTextReader);

                XmlNode roundSolved = (xml.GetElementsByTagName("RoundSolved"))[0];
                Success = bool.Parse(roundSolved["Added"].InnerText);
                RoundId = int.Parse(roundSolved["ID"].InnerText);
                ErrorMessage = roundSolved["Error"].InnerText;
                Message = roundSolved["Message"].InnerText;
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }
    }
}
