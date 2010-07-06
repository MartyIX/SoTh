using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.GameDesk
{
    public interface IQuest
    {
        string Name { get; }
        string ActualRoundXML {get;}
        string WholeQuestXml { get; }
        void MoveCurrentToNext();
        void SetCurrentRound(int n);
        bool IsLast();
    }
}
