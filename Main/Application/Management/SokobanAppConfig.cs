using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace Sokoban.Configuration
{
    class ServerElement : System.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }
    }

    class ServerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).Name;
        }
    }

    class ServerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers
        {
            get 
            {                
                return (ServerElementCollection)this["servers"];
            }            
        }
    }
}