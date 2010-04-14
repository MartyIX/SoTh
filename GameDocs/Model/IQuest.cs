using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonDock;

namespace Sokoban.Model.GameDesk
{
    public interface IQuest
    {
        string Name { get; }
        string ActualRoundXML {get;}
        void MoveCurrentToNext();
        bool IsLast();
    }
}
