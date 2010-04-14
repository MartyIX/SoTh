using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonDock;
using Sokoban.View.GameDocsComponents;

namespace Sokoban.Model.GameDesk
{
    interface IGame
    {
        void RegisterVisual(GameDeskControl gameDeskControl);
        IQuest Quest
        {
            get;
        }

        double CurrentPhase {get; }
    }
}
