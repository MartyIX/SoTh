using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Lib
{
    public class Triple<T, U, V>
    {
        public Triple()
        {
        }

        public Triple(T first, U second, V third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public T First { get; set; }
        public U Second { get; set; }
        public V Third { get; set; }
    };
}
