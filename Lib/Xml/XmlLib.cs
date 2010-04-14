using System.Windows;

namespace Sokoban.Lib.Xml
{
    public class XmlLib
    {
        /// <summary>
        /// Check if the provided XML is valid according to @xmlScheme
        /// </summary>
        /// <param name="xmlScheme"></param>
        /// <param name="validatedXml"></param>
        /// <returns></returns>
        public static bool IsXmlValid(string xmlScheme, string validatedXml)
        {
            return IsXmlValid(xmlScheme, validatedXml, true);
        }

        public static bool IsXmlValid(string xmlScheme, string validatedXml, bool showErrors)
        {
            XmlValidator xmlSchemaValidator = new XmlValidator(xmlScheme, validatedXml);

            if (!xmlSchemaValidator.IsValidXml && showErrors)
            {
                DebuggerIX.WriteLine("[XmlLib]", "[Error in XML]: " 
                    + xmlSchemaValidator.ValidationError 
                    + "\n\n[Parsed XML]: " + validatedXml);
            }

            return xmlSchemaValidator.IsValidXml;
        }
    }
}