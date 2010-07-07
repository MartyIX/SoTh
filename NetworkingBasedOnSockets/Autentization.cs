using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Networking
{
    [Serializable]
    public class Autentization
    {
        public string Name { get; set; }
        public string IP { get; set; }

        public Autentization(string name, string ip)
        {
            Name = name;
            IP = ip;
        }
    }
}
