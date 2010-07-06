using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Sokoban.View.GameDocsComponents
{
    public interface IGraphicsControl
    {
        Canvas Canvas { get; }
        double FieldSize { get; }
        void AddVisual(UIElement c);
        void ClearVisuals();
        void SetGameDeskSize(int fieldsX, int fieldsY);
    }
}
