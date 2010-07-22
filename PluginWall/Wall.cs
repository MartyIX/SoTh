using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace PluginWall
{
    public partial class Wall : IGamePlugin, IFixedElement
    {
        private object syncRoot = new object();
        private int posX = 1; // 1-based
        private int posY = 1;
        protected ScaleTransform scale;
        protected Image image;
        protected IPluginParent host;

        public Wall(IPluginParent host)
        {
            this.host = host;
        }


        #region IPlugin Members

        public string Name
        {
            get { return "Wall"; }
        }

        public string Description
        {
            get { return "Oficial Wall implementation"; }
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
            get { return "1.00"; }
        }

        public void Load()
        {
            // One-based values
            posX = 2;
            posY = 2;

            image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/PluginWall;component/Resources/obj_W.png");
            bi.EndInit();
            image.Source = bi;

            this.uiElement = image;
        }

        public void Unload()
        {
            image = null;
            uiElement = null;
        }

        public IPluginParent Parent
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }

        #endregion

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

        UIElement uiElement = null;

        public UIElement UIElement
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
            get { return PluginWall.Properties.Resources.XmlSchema; }
        }

        public void MessageReceived(object message, IGamePlugin p)
        {

        }

        public bool ProcessXmlInitialization(string gameVariant, int mazeWidth, int mazeHeight, XmlNode settings)
        {
            if (gameVariant.ToLower() != "ordinary" && gameVariant.ToLower() != "soth")
            {
                throw new Exception("Plugin Aim doesn't support game variant: " + gameVariant);
            }

            DebuggerIX.WriteLine("[Plugin]", this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);

            return true;
        }

        #region IPosition Members

        public int PosX
        {
            get
            {
                return posX;
            }
            set
            {
                posX = value;
            }
        }

        public int PosY
        {
            get
            {
                return posY;
            }
            set
            {
                posY = value;
            }
        }

        #endregion

        #region IGamePlugin Members


        public void Draw(System.Windows.Controls.Canvas canvas, double squareSize, long time, double phase)
        {
            double x = (this.posX - 1) * squareSize;
            double y = (this.posY - 1) * squareSize;


            double scaleX = squareSize / image.ActualWidth;
            double scaleY = squareSize / image.ActualHeight;

            if (scale == null || (scale.ScaleX != scaleX || scale.ScaleY != scaleY))
            {
                scale = new ScaleTransform(scaleX, scaleY);
                image.RenderTransformOrigin = new Point(0, 0);
                image.RenderTransform = this.scale;
            }

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);

            Canvas.SetZIndex(image, 30); // 10 is for tile, 11 - 19 is for middle layer objects and GameObjects have 20
        }

        public bool ProcessEvent(long time, Event e)
        {
            return true;
        }

        #endregion
    }
}
