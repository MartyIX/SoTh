using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AvalonDock;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl
    {
        /// <summary>
        /// Resize according to GameDeskControl cache
        /// </summary>
        public void Resize()
        {
            double availableWidth = this.availableWidth - this.BorderThickness.Left - this.BorderThickness.Right;
            double availableHeight = this.availableHeight - this.BorderThickness.Bottom - this.BorderThickness.Top;

            Resize(availableWidth, availableHeight);
        }

        /// <summary>
        /// Available width/height for whole GameDeskControl!!
        /// </summary>
        /// <param name="availableWidth"></param>
        /// <param name="availableHeight"></param>
        public void Resize(double availableWidth, double availableHeight)
        {
            // Cache the sizes
            this.availableWidth = availableWidth;
            this.availableHeight = availableHeight;

            double width = 0;
            double height = 0;

            if (this.displayBothDesks == false)
            {
                this.gamedeskOpponentCanvas.Visibility = System.Windows.Visibility.Collapsed;

                // Compute sizes of gamedesk
                availableWidth = availableWidth - infoPanel.ActualWidth /* margin */;
                availableWidth = (availableWidth < 0) ? 0 : availableWidth;
                availableHeight = (availableHeight < 0) ? 0 : availableHeight;

                double fieldX = Math.Floor(availableWidth / (double)fieldsX);
                double fieldY = Math.Floor(availableHeight / (double)fieldsY);
                FieldSize = Math.Min(fieldX, fieldY); // We want to notify

                width = FieldSize * fieldsX;
                height = FieldSize * fieldsY;

                this.gamedeskRect.Width = width;
                this.gamedeskRect.Height = height;
            }
            else
            {
                this.gamedeskOpponentCanvas.Visibility = System.Windows.Visibility.Visible;

                // Compute sizes of gamedesk; 10 = margin; dividing by two is because we need place for two desks
                availableWidth = (availableWidth - infoPanel.ActualWidth) / 2;
                availableWidth = (availableWidth < 0) ? 0 : availableWidth;
                availableHeight = (availableHeight < 0) ? 0 : availableHeight;

                double fieldX = Math.Floor(availableWidth / (double)fieldsX);
                double fieldY = Math.Floor(availableHeight / (double)fieldsY);
                FieldSize = Math.Min(fieldX, fieldY); // We want to notify

                width = FieldSize * fieldsX;
                height = FieldSize * fieldsY;

                this.gamedeskRect.Width = width;
                this.gamedeskRect.Height = height;

                this.gamedeskOpponentRect.Width = width;
                this.gamedeskOpponentRect.Height = height;

                /*this.gamedeskOpponentCanvas.Height = height;
                this.gamedeskCanvas.Height = height;*/
            }

            if (OnResized != null)
            {
                OnResized(width, height, FieldSize);
            }
        }

        /// <summary>
        /// Handler for event SizeChanged of GamesDocumentPane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Resize(object sender, SizeChangedEventArgs e)
        {
            if (e.Source is DocumentPane)
            {
                this.availableWidth = e.NewSize.Width;
                this.availableHeight = e.NewSize.Height;
                Resize(availableWidth, availableHeight);
            }
        }

        /// <summary>
        /// Handler for event SizeChanged of InfoPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResizeInfoPanel(object sender, SizeChangedEventArgs e)
        {
            // Resize: Data from cache but InfoPanel sizes are now correct!
            Resize(this.availableWidth, this.availableHeight);
        }
    }
}
