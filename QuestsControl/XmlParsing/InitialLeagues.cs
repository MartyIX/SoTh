using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Sokoban.Xml;
using Sokoban.Model;

namespace Sokoban.View
{
    public class InitialLeagues
    {
        private Categories categories;

        public Categories Categories
        {
            get { return categories; }
        }

        public InitialLeagues()
        {
            categories = new Categories();            
        }

        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));
            xml.Load(xmlTextReader);

            try
            {
                XmlNodeList xmlCategories = xml.SelectNodes("InitLeagues/Categories/Category");

                for (int i = 0; i < xmlCategories.Count; i++)
                {
                    XmlNode category = xmlCategories[i];
                    int category_id = int.Parse(category["ID"].InnerText);
                    bool categoryLoaded = bool.Parse(category["Loaded"].InnerText);

                    Leagues l = LeagueParsing.ParseLeagues(category.SelectNodes("Leagues/League"));
                    // Result
                    this.categories.Add(new Category(category_id, category["Name"].InnerText, categoryLoaded, l));
                }
            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }
    }

    
}
