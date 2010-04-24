using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.PluginInterface
{
    public interface IFixedObject
    {
        int PosX { get; set; }
        int PosY { get; set; }
    }
}
