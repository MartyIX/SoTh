using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model
{
    public interface IGameRealTime
    {
        double CurrentPhase { get; }
    }
}
