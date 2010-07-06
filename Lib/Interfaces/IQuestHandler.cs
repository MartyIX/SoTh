using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;

namespace Sokoban.Model.Quests
{
    public interface IQuestHandler
    {
        void QuestSelected(IQuest quest, GameMode gameMode);
    }
}
