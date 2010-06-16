using System.Windows;

namespace Sokoban.Lib.Xml
{
    public class XmlLib
    {
        /// <summary>
        /// Check if the provided XML is valid according to @xmlScheme
        /// </summary>
        /// <param name="xmlScheme"></param>
        /// <param name="xmlInstance"></param>
        /// <returns></returns>
        public static bool IsXmlValid(string xmlScheme, string xmlInstance)
        {
            XmlValidator xmlSchemaValidator = new XmlValidator();
            return xmlSchemaValidator.IsValid(null, xmlScheme, xmlInstance);
        }
    }
}