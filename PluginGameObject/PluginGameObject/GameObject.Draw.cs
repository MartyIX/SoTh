#define DEBUG_COMPUTING
#define DEBUG_MOVEMENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Sokoban.Lib;

namespace Sokoban.Model.Plugins
{
    public partial class GameObject
    {
        protected float lastDrawedX = 0;
        protected Stopwatch redrawStopWatch = new Stopwatch();
        protected ScaleTransform scale;


        public virtual void Draw(Canvas canvas, double squareSize, Int64 time, double phase)
        {
            // 50x50 is maximal expected size of image
            double expectedSize = 50;

            double fieldSizeScaleX = UI.Image.ActualWidth / expectedSize;
            double fieldSizeScaleY = UI.Image.ActualHeight / expectedSize;

            double centerXpos = squareSize * (1 - fieldSizeScaleX) / 2;
            double centerYpos = squareSize * (1 - fieldSizeScaleY) / 2;

            double x;
            double y;

            if (MovementInProgress(time) == false)
            {
                x = (this.posX - 1) * squareSize + centerXpos;
                y = (this.posY - 1) * squareSize + centerYpos;

                if (Description == "S")
                {
                    DebuggerIX.WriteLine("[Draw]", "NotInProgress",
                          "[x,y] = " + x.ToString("0.00")
                        + ", " + y.ToString("0.00")
                        + "; pos = " + posX.ToString() + "x" + posY.ToString()
                        + "; phase = " + phase.ToString("0.0000"));
                }
            }
            else
            {
                double startTime = (double)UI.MovementStartTime + UI.MovementStartTimePhase;
                double timePassed = ((double)time + phase - startTime);
                double progress = timePassed / (double)(this.Speed - UI.MovementStartTimePhase);
                double step = squareSize * progress;

                x = (this.lastPosX - 1) * squareSize + centerXpos + (posX - lastPosX) * step;
                y = (this.lastPosY - 1) * squareSize + centerYpos + (posY - lastPosY) * step;

                if (Description == "S")
                {
                    DebuggerIX.WriteLine("[Draw]", "InProgress",
                          "[x,y] = " + x.ToString("0.00")
                        + ", " + y.ToString("0.00")
                        + "; pos = " + lastPosX.ToString() + "x" + lastPosY.ToString()
                        + " -> " + posX.ToString() + "x" + posY.ToString()
                        + "; SS = " + squareSize.ToString()
                        + "; MST = " + startTime.ToString("0.000")
                        + "; Time = " + time.ToString() + phase.ToString(".0000")
                        + "; progress = " + progress.ToString("0.000")
                        + "; step = " + step.ToString()
                        + "; TimeMovementEnd = " + UI.MovementEndTime.ToString());
                }

            }


            double scaleX = (squareSize * fieldSizeScaleX) / UI.Image.ActualWidth;
            double scaleY = (squareSize * fieldSizeScaleY) / UI.Image.ActualHeight;

            if (scale == null || (scale.ScaleX != scaleX || scale.ScaleY != scaleY))
            {
                scale = new ScaleTransform(scaleX, scaleY);
                UI.Image.RenderTransform = this.scale;
            }

            if (Description == "M")
            {
                //Deb.WriteLine(phase, x, y);
            }

            Canvas.SetLeft(UI.Image, x);
            Canvas.SetTop(UI.Image, y);

        }
    }
}
