using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public interface IGraphicsControl
    {
        Canvas Canvas { get; }
        double FieldSize { get; }
        void AddVisual(UIElement c);
        void ClearVisuals();
        void SetGameDeskSize(int fieldsX, int fieldsY);
        void StopTime();
        void RestartGame(GameDisplayType gameDisplayType); // which player
        void GameChangedHandler(GameDisplayType gameDisplayType, GameChange gameChange);
        void PauseTime();
        void PlayNextRound();
    }
}
