#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Media;
using System.Net;
using System.Xml;
using System.Xml.Schema;
#endregion

namespace Sokoban
{
    /// <summary>
    /// This class validates an xml string or xml document against an xml schema.
    /// It has public methods that return a boolean value depending on the validation
    /// of the xml.
    /// </summary>
    public class XmlValidator
    {
		#region Fields (2) 

        private bool isValidXml_;
        private string validationError_ = "";

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Constructor. Scheme file is stored in resources
        /// </summary>
        public XmlValidator(string xmlScheme, string xmlFile)
        {
            isValidXml_ = true;
            XmlTextReader xmlSchemeReader = null;
            XmlTextReader xmlFileReader = null;
            XmlDocument document;

            try
            {
                xmlSchemeReader = new XmlTextReader(new StringReader(xmlScheme));
                xmlFileReader = new XmlTextReader(new StringReader(xmlFile));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xmlSchemeReader);
                settings.ValidationType = ValidationType.Schema;
                document = new XmlDocument();
                document.Load(xmlFileReader); // xml file

                XmlReader rdr = XmlReader.Create(new StringReader(document.InnerXml), settings);

                try
                {
                    while (rdr.Read())
                    {
                    }
                }
                catch (XmlSchemaValidationException e)
                {
                    validationError_ = e.Message;
                    isValidXml_ = false;
                }
            }
            catch (XmlSchemaValidationException e)
            {
                validationError_ = e.Message;
                isValidXml_ = false;
            }
            finally
            {
                xmlSchemeReader.Close();
                xmlFileReader.Close();
            }
        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// True if XML is valid otherwise false
        /// </summary>
        public bool IsValidXml
        {
            get { return isValidXml_; }
            set { isValidXml_ = value; }
        }

        /// <summary>
        /// Errors that happen during validation. Interesting only if IsValidXml is false.
        /// </summary>
        public string ValidationError
        {
            get { return validationError_; }
            set { validationError_ = value; }
        }

		#endregion Properties 
    }
}