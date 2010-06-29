using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Globalization;

namespace Sokoban.View
{
    public class SolverPainter
    {
        private Canvas canvas;
        private int tailShortenBy;
        private string solution;
        private Point startPoint;
        private List<Point> points = null;
        private double fieldSize;
        private Path finalPath = null;
        private Path finalArrow = null;
        private int sokobanX;
        private int sokobanY;
        private bool isUpdateEnabled;

        // Appearance
        private SolidColorBrush solidColorBrush;
        private double strokeThickness = 2;
        private PenLineJoin strokeLineJoin = PenLineJoin.Round;
        private int zIndex = 15;
        private double hidingLength = 0.75; // hiding of a line segment when sokoban goes "the right way"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="sokobanX">One-based x-coordinate</param>
        /// <param name="sokobanY">One-based y-coordinate</param>
        /// <param name="fieldSize"></param>
        /// <param name="solution"></param>
        public SolverPainter(Canvas canvas, int sokobanX, int sokobanY, double fieldSize, string solution)
        {
            sokobanX--;
            sokobanY--;

            this.isUpdateEnabled = true;

            this.solidColorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            this.sokobanX = sokobanX;
            this.sokobanY = sokobanY;
            this.fieldSize = fieldSize;

            this.computeStartPoint();

            this.canvas = canvas;
            this.tailShortenBy = 0;
            this.solution = solution;
        }

        public void Draw()
        {
            if (points == null)
            {
                this.preparePoints();
            }

            this.Terminate();
            
            finalPath = new Path(); 
            PathGeometry finalPathGeometry = new PathGeometry(); 
            PathFigure primaryFigure = new PathFigure();

            //if you want the path to be a shape, you want to close the PathFigure 
            //   that makes up the Path. If you want it to simply by a line, set 
            //   PrimaryFigure.IsClosed = false; 
            primaryFigure.IsClosed = false;     
            primaryFigure.StartPoint = this.points[tailShortenBy];


            if (this.tailShortenBy < solution.Length) // check if we are not out of bounds
            {

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();
                Point lastPoint = primaryFigure.StartPoint;
                char lastDirection = solution[this.tailShortenBy];

                if (!(lastDirection == 'L' || lastDirection == 'R' || lastDirection == 'U' || lastDirection == 'D')) // moving of a box
                {
                    for (int i = this.tailShortenBy + 1; i < solution.Length; i++)
                    {
                        LineSegment newSegment = new LineSegment();
                        newSegment.Point = points[i];
                        LineSegmentCollection.Add(newSegment);

                        lastPoint = newSegment.Point;

                        // Box

                        lastDirection = solution[i];

                        if (solution[i] == 'L' || solution[i] == 'R' || solution[i] == 'U' || solution[i] == 'D') break;
                    }
                }

                primaryFigure.Segments = LineSegmentCollection;

                // Completing results
                PathFigureCollection pfc = new PathFigureCollection(5);
                pfc.Add(primaryFigure);
                finalPathGeometry.Figures = pfc;

                finalPath.Data = finalPathGeometry;
                finalPath.StrokeLineJoin = this.strokeLineJoin;

                finalPath.Stroke = this.solidColorBrush;
                finalPath.StrokeThickness = this.strokeThickness;

                this.addToCanvas(finalPath);
                drawArrow(lastPoint, lastDirection);
            }
            else
            {
                this.Terminate();
            }
        }


        /// <summary>
        /// Please check if your CurrentCulture uses decimal points in double.ToString() otherwise the function doesn't work.
        /// </summary>
        /// <param name="arrowTip"></param>
        /// <param name="direction"></param>
        private void drawArrow(Point arrowTip, char direction)
        {
            int arrowWidth = Convert.ToInt32(Math.Round(fieldSize / 6));
            int arrowHeight = Convert.ToInt32(Math.Round(fieldSize / 10));
            int arrowTail = 3* arrowWidth;

            StringBuilder arrowStr = new StringBuilder(30); // initial capacity

            arrowStr.Append("M " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");            

            direction = Char.ToLower(direction);

            if (direction == 'l')
            {
                arrowTip.X -= arrowTail;
                arrowStr.Append("L " + arrowTip.X.ToString() 
                    + " " + arrowTip.Y.ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X + arrowWidth).ToString() + " " + (arrowTip.Y + arrowHeight).ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X + arrowWidth).ToString() + " " + (arrowTip.Y - arrowHeight).ToString() + " ");
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
            }
            else if (direction == 'r')
            {
                arrowTip.X += arrowTail;
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X - arrowWidth).ToString() + " " + (arrowTip.Y + arrowHeight).ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X - arrowWidth).ToString() + " " + (arrowTip.Y - arrowHeight).ToString() + " ");
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
            }
            else if (direction == 'u')
            {
                arrowTip.Y -= arrowTail;
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X + arrowHeight).ToString() + " " + (arrowTip.Y + arrowWidth).ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X - arrowHeight).ToString() + " " + (arrowTip.Y + arrowWidth).ToString() + " ");
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
            }
            else if (direction == 'd')
            {
                arrowTip.Y += arrowTail;
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X - arrowHeight).ToString() + " " + (arrowTip.Y - arrowWidth).ToString() + " ");
                arrowStr.Append("L " + (arrowTip.X + arrowHeight).ToString() + " " + (arrowTip.Y - arrowWidth).ToString() + " ");
                arrowStr.Append("L " + arrowTip.X.ToString() + " " + arrowTip.Y.ToString() + " ");
            }

            PathFigureCollection arrowPFC = ((PathFigureCollection)new PathFigureCollectionConverter().ConvertFromString(arrowStr.ToString()));

            PathGeometry finalArrowGeometry = new PathGeometry();
            finalArrowGeometry.Figures = arrowPFC;

            finalArrow = new Path(); 
            finalArrow.Data = finalArrowGeometry;
            finalArrow.StrokeLineJoin = this.strokeLineJoin;
            finalArrow.StrokeThickness = this.strokeThickness;            
            finalArrow.Fill = this.solidColorBrush;
            finalArrow.Stroke = this.solidColorBrush;

            this.addToCanvas(finalArrow);
        }


        public void Update(int newX, int newY, char direction)
        {
            if (isUpdateEnabled == true)
            {
                // User went in the direction of the our line
                if (this.tailShortenBy < solution.Length && direction == Char.ToLower(solution[tailShortenBy]))
                {
                    sokobanX = newX;
                    sokobanY = newY;

                    ShortenTail();
                }
                else
                {
                    this.Terminate(); // User didn't follow the solution or we're at the end of solution -> discard the helper line 
                    isUpdateEnabled = false;
                }
            }
        }

        public void ShortenTail()
        {
            ShortenTail(1);
        }

        public void ShortenTail(int fields)
        {
            tailShortenBy += fields;

            PathGeometry pg = finalPath.Data as PathGeometry;

            if (pg.Figures[0].Segments.Count < fields)
            {
                this.Terminate(); // hide arrow
                this.Draw();
            }
            else
            {
                for (int i = 0; i < fields; i++)
                {
                    Point from = pg.Figures[0].StartPoint;
                    Point to = ((LineSegment)(pg.Figures[0].Segments[0])).Point;
                    runShortenTailAnimation(from, to);
                    pg.Figures[0].StartPoint = to;
                    pg.Figures[0].Segments.RemoveAt(0); // remove leading line segments
                }
            }

            //this.Draw(); // TODO REMOVE
        }

        /// <summary>
        /// Hides path created from points p1 and p2
        /// </summary>
        /// <param name="p1">From</param>
        /// <param name="p2">To</param>
        private void runShortenTailAnimation(Point p1, Point p2)
        {
            string data = "M " + p1.X.ToString() + " " + p1.Y.ToString() + " " + "L " + p2.X.ToString() + " " + p2.Y.ToString();

            PathFigureCollection pfc = ((PathFigureCollection)new PathFigureCollectionConverter().ConvertFromString(data));

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = pfc;

            Path path = new Path();
            path.Data = pathGeometry;
            path.StrokeLineJoin = this.strokeLineJoin;
            path.StrokeThickness = this.strokeThickness;
            path.Fill = this.solidColorBrush;
            path.Stroke = this.solidColorBrush;

            this.addToCanvas(path);

            // We want to hide the line segment
            DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(this.hidingLength)));
            path.BeginAnimation(Path.OpacityProperty, doubleAnimation);           
        }

        public void Redraw(double fieldSize)
        {
            if (fieldSize != this.fieldSize)
            {
                this.fieldSize = fieldSize;
                this.computeStartPoint();
                this.preparePoints();
                this.Draw();
            }
        }

        private void addToCanvas(UIElement uiElement)
        {
            // Add to the canvas
            canvas.Children.Add(uiElement);
            Canvas.SetLeft(uiElement, 0);
            Canvas.SetTop(uiElement, 0);
            Canvas.SetZIndex(uiElement, this.zIndex); // 10 is for tiles, 11-19 for middle layers and 20 is for objects        
        }

        private void computeStartPoint()
        {
            this.startPoint = new Point(sokobanX * fieldSize + fieldSize / 2, sokobanY * fieldSize + fieldSize / 2);
        }

        private void preparePoints()
        {
            points = new List<Point>();

            points.Add(this.startPoint);

            Point lastPoint = this.startPoint;
            double diffX = 0, diffY = 0;

            for (int i = 0; i < this.solution.Length; i++)
            {
                if (this.solution[i] == 'l' || this.solution[i] == 'L')
                {
                    diffX = - this.fieldSize;
                    diffY = 0;
                }
                else if (this.solution[i] == 'r' || this.solution[i] == 'R')
                {
                    diffX = this.fieldSize;
                    diffY = 0;

                }
                else if (this.solution[i] == 'u' || this.solution[i] == 'U')
                {
                    diffX = 0;
                    diffY = - this.fieldSize;
                }
                else if (this.solution[i] == 'd' || this.solution[i] == 'D')
                {
                    diffX = 0;
                    diffY = this.fieldSize;
                }

                Point point = new Point(lastPoint.X + diffX, lastPoint.Y + diffY);
                lastPoint = point;

                points.Add(point);
            }
        }

        public void Terminate()
        {
            if (finalPath != null)
            {
                this.canvas.Children.Remove(finalPath);
            }

            if (finalArrow != null)
            {
                this.canvas.Children.Remove(finalArrow);
            }
        }
    }
}
