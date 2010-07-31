using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace Sokoban.View.GameDocsComponents
{
    public class Fader : Adorner
    {
        private Brush brush;
        private Pen pen = new Pen(Brushes.Gray, 1);
        private Point corner = new Point(0, 0);
        private string text;
        private Typeface typeface = new Typeface(new FontFamily("Verdana"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
        private Point center;
        private FormattedText formattedText;

        public Fader(UIElement uiElement, Color backgroundColor, string text) : base(uiElement)
        {
            brush = new SolidColorBrush(backgroundColor);
            this.text = text;

            formattedText = new FormattedText(this.text, Thread.CurrentThread.CurrentUICulture,
                FlowDirection.LeftToRight, typeface, 30, System.Windows.Media.Brushes.White);          
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(brush, pen, new Rect(corner, DesiredSize));
            center = new Point((DesiredSize.Width - formattedText.Width)/2, (DesiredSize.Height - formattedText.Height)/2);            
            drawingContext.DrawText(formattedText, center);                        
            base.OnRender(drawingContext);
        }
    }
}
