using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using Sokoban.Model.GameDesk;

namespace Sokoban.Model.GameDesk
{
    /// <summary>
    /// Discrete simulation for actual round
    /// </summary>
    public class IsPermitted<T>
    {
        private bool[] isPermitted;

        public IsPermitted()
        {
            isPermitted = new bool[Enum.GetValues(typeof(T)).Length];

            for (int i = 0; i < isPermitted.Length; ++i)
                isPermitted[i] = true;
        }

        /// <summary>
        /// Gets or sets if an EventType is permitted or not to process in this.Move() method
        /// </summary>
        /// <param name="et"></param>
        /// <returns>true if EventType is permitted; otherwise false</returns>
        public bool this[EventType et]
        {
            get { return isPermitted[(int)et - 1]; }
            set { isPermitted[(int)et - 1] = value; }
        }
    }
}
