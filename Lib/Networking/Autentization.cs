using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Networking
{
    [Serializable]
    public class Authentication  
    {
        public string Name { get; set; }
        public string IP { get; set; }

        public Authentication(string name, string ip)
        {
            Name = name;
            IP = ip;
        }

        public override string ToString()
        {
            return Name + " (" + IP + ")";
        }
    }
}
