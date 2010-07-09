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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="league_id">ID to the table leagues_id</param>
        /// <param name="round_id">ID to the table rounds_id; -1 if only league was chosen</param>
        /// <param name="quest">Quest instance</param>
        /// <param name="gameMode"></param>
        void QuestSelected(int leaguesID, int roundsID, IQuest quest, GameMode gameMode);
    }
}
