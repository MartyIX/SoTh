using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace Sokoban.View.GameDocsComponents
{
    public class GraphicsControl : IGraphicsControl
    {
        private Canvas canvas = null;
        private Rectangle gamedesk;
        private GameDeskControl gameDeskControl;

        public GraphicsControl(GameDeskControl gameDeskControl, Canvas canvas, Rectangle gamedesk)
        {
            this.canvas = canvas;
            this.gamedesk = gamedesk;
            this.gameDeskControl = gameDeskControl;
        }

        #region IGraphicsControl Members

        public Canvas Canvas
        {
            get { return canvas; }
        }

        public double FieldSize
        {
            get { return this.gameDeskControl.FieldSize; }
        }

        public void AddVisual(UIElement c)
        {
            canvas.Children.Add(c);
        }

        public void ClearVisuals()
        {
            canvas.Children.Clear();
            this.AddVisual(gamedesk);
        }

        /// <summary>
        /// Changes control appearance
        /// </summary>
        /// <param name="fieldsX"></param>
        /// <param name="fieldsY"></param>
        public void SetGameDeskSize(int fieldsX, int fieldsY)
        {
            gameDeskControl.fieldsX = fieldsX;
            gameDeskControl.fieldsY = fieldsY;

            gameDeskControl.Resize();
        }

        #endregion
    }
}
