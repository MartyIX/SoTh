using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PluginMonster
{
    public class Monster : MovableEssentials, IGamePlugin, IMovableElement
    {
        private bool wasSokobanCatched = false;
        private Int64 timeOfDeath;
        private UIElement uiElement;
        private Image stainImage;
        private IPluginParent parent;
        private IGamePlugin sokoban;

        private int[,] wave = null;
        private ArrayList waveQueue = null;
        private static int[] moves = { -1,  0, 
                                        1,  0, 
                                        0, -1, 
                                        0,  1 };
        


        public Monster(IPluginParent host) : base(host)
        {
            parent = host;
            Initialize(this);
        }


        #region IGamePlugin Members

        public string Name
        {
            get { return "Monster"; }
        }

        public string Description
        {
            get { return "Official implementation of Monster object for `SoTh' game variant."; }
        }

        public string Author
        {
            get { return "Martin Vseticka"; }
        }

        public string Version
        {
            get { return "1.00"; }
        }

        public string CreatedForHostVersion
        {
            get { return "2.00"; }
        }

        public System.Windows.UIElement UIElement
        {
            get
            {
                return uiElement;
            }
            set
            {
                uiElement = value;
            }
        }

        public string XmlSchema
        {
            get { return PluginMonster.Properties.Resources.XmlSchema; }
        }

        public void Load(string appPath)
        {
            //
            // Textures
            //
            

            image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/PluginMonster;component/Resources/obj_M.png");
            bi.EndInit();
            image.Source = bi;

            this.uiElement = image;

            foreach (IGamePlugin gp in host.AllPlugins)
            {
                if (gp.Name == "Sokoban")
                {
                    this.sokoban = gp;
                    break;
                }
            }

            if (this.sokoban == null)
            {
                throw new PluginLoadFailedException("There's no Sokoban plugin on game desk.");
            }
            
            obstructionLevel = - 1; // it's not an obstruction at all! Special value
            this.Speed = 7;
        }

        public void Unload()
        {
            
        }

        public bool ProcessXmlInitialization(string gameVariant, int mazeWidth, int mazeHeight, System.Xml.XmlNode settings)
        {
            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);
            fieldsX = mazeWidth;
            fieldsY = mazeHeight;
            
            host.MakeImmediatePlan("MonsterInitialization", this, (EventType)Enum.Parse(typeof(EventType), settings["FirstState"].InnerText));

            return true;
        }

        public void MessageReceived(string messageType, object message, IGamePlugin sender)
        {
            
        }

        int id = 0;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        #endregion

        private void ProcessPursuitEvent(long time, Event ev)
        {
        
            if (this.wave == null) wave = new int[fieldsX, fieldsY];
            if (this.waveQueue == null) waveQueue = new ArrayList(50);

            for (int j = 0; j < fieldsY; j++)
            {
                for (int i = 0; i < fieldsX; i++)
                {
                    wave[i, j] = -1;
                }
            }

            wave[this.posX - 1, this.posY - 1] = 0; // start
            waveQueue.Clear();
            waveQueue.Add(new Coordinate(this.posX, this.posY));

            int g = 0;
            int akt = 0;
            // One-based values
            int pSx = sokoban.PosX;
            int pSy = sokoban.PosY;

            while (g < waveQueue.Count)
            {
                Coordinate tmp = (Coordinate)waveQueue[g];
                akt = wave[tmp.x - 1, tmp.y - 1];

                if (tmp.x == pSx && tmp.y == pSy)
                {
                    break;
                }

                for (int t = 0; t < 4; t++)
                {
                    int newX = tmp.x + moves[2 * t];
                    int newY = tmp.y + moves[2 * t + 1];
                    IGamePlugin obstruction = host.GetObstructionOnPosition(newX, newY);

                    if (obstruction == null || obstruction.Name == "Sokoban")
                    {
                        if (wave[newX - 1, newY - 1] == -1)
                        {
                            wave[newX - 1, newY - 1] = akt + 1; // start
                            waveQueue.Add(new Coordinate(newX, newY));
                        }
                    }
                }
                g++;
            }

            waveQueue.Clear();

            // REKONSTRUKCE CESTY
            // ================================================================================
            akt = wave[pSx - 1, pSy - 1];

            if (akt > -1) // akt == -1 znaci, ze neexistuje cesta k sokobanovi
            {
                Random rndNum = new Random();

                while (wave[pSx - 1, pSy - 1] != 1)
                {

                    if (wave[pSx - 1, pSy - 1] < 1)
                    {
                        break;
                    }

                    int[] tmpSmery = new int[] { 0, 1, 2, 3 };
                    int xTmpSmery = 3;

                    for (int u = 0; u < 4; u++)
                    {
                        int nahodne = rndNum.Next(0, xTmpSmery);
                        int t = tmpSmery[nahodne];
                        tmpSmery[nahodne] = tmpSmery[xTmpSmery];
                        xTmpSmery--;

                        int newX = pSx + moves[2 * t];
                        int newY = pSy + moves[2 * t + 1];

                        if (newX - 1 >= 0 && newY - 1 >= 0 &&
                            newX - 1 < fieldsX && newY - 1 < fieldsY &&
                            wave[newX - 1, newY - 1] < akt && wave[newX - 1, newY - 1] != -1)
                        {
                            pSx = newX;
                            pSy = newY;
                            akt = wave[newX - 1, newY - 1];
                            break;
                        }
                    }
                }
            }

            if (akt == 1)
            {
                if (pSx - this.posX < 0)
                {
                    host.MakePlan("MonsterGoXXX", time + this.Speed, this, EventType.goLeft);
                }
                else if (pSx - this.posX > 0)
                {
                    host.MakePlan("MonsterGoXXX", time + this.Speed, this, EventType.goRight);
                }
                else if (pSy - this.posY < 0)
                {
                    host.MakePlan("MonsterGoXXX", time + this.Speed, this, EventType.goUp);
                }
                else if (pSy - this.posY > 0)
                {
                    host.MakePlan("MonsterGoXXX", time + this.Speed, this, EventType.goDown);
                }
            }

            host.MakePlan("MonsterGoXXX", time + this.Speed, this, EventType.pursuit);

        }

        public override void Crash(Canvas canvas, Int64 time, double monLeft, double monTop, double squareSize)
        {
            double sokLeft = Canvas.GetLeft(sokoban.UIElement);
            double sokTop = Canvas.GetTop(sokoban.UIElement);            
            
            if (wasSokobanCatched == false)
            {

                double addition = squareSize * 0.35;
                double size = squareSize * (1 - 0.35 * 2);

                double realSokLeft = sokLeft + addition;
                double realSokTop = sokTop + addition;

                double realMonLeft = monLeft + addition;
                double realMonTop = monTop + addition;


                Rect r1 = new Rect(realSokLeft, realSokTop, size, size);
                Rect r2 = new Rect(realMonLeft, realMonTop, size, size);

                if (r1.IntersectsWith(r2))
                {
                    timeOfDeath = time;

                    this.host.MakeImmediatePlan("MonsterStopTime", null, EventType.stopCountingTime);
                    this.host.StopSimulation();

                    wasSokobanCatched = true;

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri("pack://application:,,,/PluginMonster;component/Resources/blood_stain_bigger.png");
                    bi.EndInit();

                    stainImage = new Image();
                    stainImage.Source = bi;
                    stainImage.Opacity = 0.0;
                    Canvas.SetLeft(stainImage, sokLeft);
                    Canvas.SetTop(stainImage, sokTop);
                    Canvas.SetZIndex(stainImage, 20);
                    canvas.Children.Add(stainImage);
                    stainImage.RenderTransformOrigin = ((Image)sokoban.UIElement).RenderTransformOrigin;
                    stainImage.RenderTransform = ((Image)sokoban.UIElement).RenderTransform;



                    // hide Sokoban && monster
                    DoubleAnimation doubleHideSokobanAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(1.5)));
                    DoubleAnimation doubleHideMonsterAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
                    DoubleAnimation doubleShowAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(1.5)));
                    //doubleHideAnimation.BeginTime = new TimeSpan(0, 0, 0, 0);
                    //doubleShowAnimation.BeginTime = new TimeSpan(0, 0, 0, 500);

                    Storyboard sb = new Storyboard();
                    sb.Duration = new Duration(TimeSpan.FromSeconds(2));
                    sb.Completed += new EventHandler(sb_Completed);

                    Storyboard.SetTarget(doubleHideSokobanAnimation, (Image)sokoban.UIElement);
                    Storyboard.SetTargetProperty(doubleHideSokobanAnimation, new PropertyPath(Image.OpacityProperty));

                    Storyboard.SetTarget(doubleHideMonsterAnimation, (Image)this.UIElement);
                    Storyboard.SetTargetProperty(doubleHideMonsterAnimation, new PropertyPath(Image.OpacityProperty));

                    Storyboard.SetTarget(doubleShowAnimation, this.stainImage);
                    Storyboard.SetTargetProperty(doubleShowAnimation, new PropertyPath(Image.OpacityProperty));

                    sb.Children.Add(doubleHideSokobanAnimation);
                    sb.Children.Add(doubleHideMonsterAnimation);
                    sb.Children.Add(doubleShowAnimation);

                    sb.Begin();
                    
                }
            }
            else
            {
                Canvas.SetLeft(stainImage, sokLeft);
                Canvas.SetTop(stainImage, sokTop);
                stainImage.RenderTransformOrigin = ((Image)sokoban.UIElement).RenderTransformOrigin;
                stainImage.RenderTransform = ((Image)sokoban.UIElement).RenderTransform;
            }
        }

        private void sb_Completed(object sender, EventArgs e)
        {            
            this.host.GameVariant.CheckRound(timeOfDeath, "SokobanKilled", 0);
        }


        public new bool ProcessEvent(Int64 time, Event ev)
        {
            bool returnValue = false;

            switch (ev.what)
            {
                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    base.SetOrientation(ev.what);

                    DebuggerIX.WriteLine(DebuggerTag.SimulationEventHandling, "[Sokoban] ProcessEvent", ev.what.ToString() + "; Raised from EventID: " + ev.EventID.ToString());
                    DebuggerIX.WriteLine(DebuggerTag.SimulationEventHandling, "", "[Sokoban]" + ev.ToString());

                    IGamePlugin obstruction = base.ProcessGoEvent(time, ev);

                    /*if (obstruction != null && obstruction.Name == "Sokoban")
                    {
                        this.host.GameVariant.CheckRound(time, "SokobanKilled", this.Speed);
                    }*/

                    returnValue = true;

                    break;


                case EventType.pursuit:

                    ProcessPursuitEvent(time, ev);

                    return true;

                default:
                    returnValue = base.ProcessEvent(time, ev);
                    break;
            }

            return returnValue;
        }


    }

    /// <summary>
    /// Helper class for storing Cartesian coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// x-coordinate
        /// </summary>
        public int x;
        /// <summary>
        /// y-coordinate
        /// </summary>
        public int y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

}
