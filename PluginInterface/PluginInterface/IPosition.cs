using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.PluginInterface
{
    public interface IPosition
    {
        /// <summary>
        /// One-based x-coord
        /// </summary>
        int PosX { get; set; }
        /// <summary>
        /// One-based y-coord
        /// </summary>
        int PosY { get; set; }
    }
}
