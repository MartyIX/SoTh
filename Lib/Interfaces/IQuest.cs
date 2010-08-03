using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.GameDesk
{
    public interface IQuest
    {
        string Name { get; }
        string CurrentRoundXML {get;}
        int CurrentRoundID { get; }
        string WholeQuestXml { get; }
        void MoveCurrentToNext();
        void SetCurrentRound(int n);
        bool IsLast();
        int RoundsNo { get; }
        bool IsLeague { get; }
        bool StoreResult(TimeSpan time, string solution, out string message);
    }
}
