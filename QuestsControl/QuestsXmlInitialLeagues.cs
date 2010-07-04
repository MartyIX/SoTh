using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Sokoban.Lib.Exceptions;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Sokoban.View
{
    public class QuestsXmlInitialLeagues
    {
        private Categories categories;

        public Categories Categories
        {
            get { return categories; }
        }

        public QuestsXmlInitialLeagues()
        {
            categories = new Categories();            
        }

        public void Parse(string response)
        {
            XmlDocument xml = new XmlDocument(); //* create an xml document object.
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(response));
            xml.Load(xmlTextReader);


            //XmlNamespaceManager names = new XmlNamespaceManager(xml.NameTable);
            //names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");

            try
            {
                XmlNodeList xmlCategories = xml.SelectNodes("InitLeagues/Category");

                for (int i = 0; i < xmlCategories.Count; i++)
                {
                    XmlNode category = xmlCategories[i];
                    int category_id = int.Parse(category["ID"].InnerText);

                    Leagues l = new Leagues();
                    XmlNodeList leaguesList = category.SelectNodes("League");

                    foreach (XmlNode league in leaguesList)
                    {
                        int id = int.Parse(league["ID"].InnerText);
                        l.Add(new League(id, league["Name"].InnerText));
                    }
                    
                    // Result
                    this.categories.Add(new Category(category_id, category["Name"].InnerText, l));
                }

            }
            catch
            {
                throw new InvalidStateException("Answer from the server is not well-formed.");
            }
        }

    }


    public class League
    {
        public League(int id, string name)
        {
            Name = name;
            ID = id;
        }

        public string Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
    }

    public class Leagues : ObservableCollection<League> { }

    public class Category
    {
        public Category(int id, string name, Leagues leagues)
        {
            ID = id;
            Name = name;
            Leagues = leagues;
        }

        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Leagues Leagues
        {
            get;
            set;
        }
    }

    public class Categories : ObservableCollection<Category> { }
}
