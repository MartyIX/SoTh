using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
namespace Sokoban.Lib
{

    public class XmlValidator
    {
        bool isValid;
        StringBuilder errorMessage = null;
        XmlReaderSettings xmlReaderSettings;
        XmlSchemaSet xmlSchemaSet;

        public XmlValidator()
        {
            xmlSchemaSet = new XmlSchemaSet();

            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(validationEventHandler);
            xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            xmlReaderSettings.Schemas = xmlSchemaSet;
        }

        public string GetErrorMessage()
        {
            return errorMessage.ToString();
        }

        private void validationEventHandler(object sender, ValidationEventArgs e)
        {
            if (errorMessage == null)
            {
                errorMessage = new StringBuilder(200);
            }

            errorMessage.AppendLine(e.Message);

            isValid = false;

            if (e.Severity == XmlSeverityType.Warning)
            {
                DebuggerIX.WriteLine("[XmlValidation]", "Warning", e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                DebuggerIX.WriteLine("[XmlValidation]", "ERROR", e.Message);
            }
        }
        
        public void AddSchema(string ns, string schema)
        {
            XmlTextReader xmlSchemaReader = new XmlTextReader(new StringReader(schema));
            xmlSchemaSet.Add(ns, xmlSchemaReader);
        }

        public bool IsValid(string ns, string schema, string xmlInstance)
        {
            this.AddSchema(ns, schema);
            return this.IsValid(xmlInstance);
        }

        public bool IsValid(string xmlInstance)
        {
            isValid = true;

            XmlTextReader instance = new XmlTextReader(new StringReader(xmlInstance));            
            XmlReader reader = XmlReader.Create(instance, xmlReaderSettings);

            try
            {
                while (reader.Read()) ;               
            }
            //This code catches any XML exceptions.
            catch (XmlException XmlExp)
            {
                DebuggerIX.WriteLine("[XmlValidation]", XmlExp.Message);
                isValid = false;
            }

            return isValid;
        }
    }
}
