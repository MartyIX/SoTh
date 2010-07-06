using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View.GameDocsComponents;
using Sokoban.Solvers;

namespace Sokoban.Model.GameDesk
{
    public interface IGame : ISolverPainter
    {
        void RegisterVisual(IGraphicsControl graphicsControl);
        IQuest Quest { get; }
        //int StepsCount { get; }
        string RoundName { get; }
        IGameRepository GameRepository { get; }
    }
}
