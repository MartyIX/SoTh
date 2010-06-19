﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace PluginAim
{
    public partial class Aim : IGamePlugin, IFixedElement
    {
        private object syncRoot = new object();
        protected int obstructionLevel = 0;
        protected int posX;
        protected int posY;
        protected IPluginParent host;
        protected Rectangle rect = null;

        public Aim(IPluginParent host)
        {
            this.host = host;
        }


        #region IPlugin Members

        public string Name
        {
            get { return "Aim"; }
        }

        public string Description
        {
            get { return "Oficial Aim implementation"; }
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

        public new bool ProcessEvent(Int64 time, Event ev)
        {
            return true; // TODO
        }

        public void Load()
        {
            // One-based values
            posX = 2;
            posY = 2;
            obstructionLevel = 0;

            // Rectangle
            rect = new System.Windows.Shapes.Rectangle();

            // Add color
            SolidColorBrush solidBrush = new SolidColorBrush();
            solidBrush.Color = Color.FromArgb(255, 0, 0, 0);
            rect.Fill = solidBrush;
        }

        public void Unload()
        {
            this.rect = null;
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

        

        public UIElement UIElement
        {
            get
            {
                return (UIElement)this.rect;
            }
            set
            {
                throw new Exception("Plugin appearance is choosen to be fixed by author.");
            }
        }

        public string XmlSchema
        {
            get { return PluginAim.Properties.Resources.XmlSchema; }
        }


        public bool ProcessXmlInitialization(int mazeWidth, int mazeHeight, XmlNode settings)
        {
            DebuggerIX.WriteLine("[Plugin]", this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);

            return true;
        }

        public int ObstructionLevel
        {
            get { return obstructionLevel; }
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
            double x;
            double y;

            x = (this.posX - 1) * squareSize;
            y = (this.posY - 1) * squareSize;

            rect.Width = squareSize;
            rect.Height = squareSize;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            Canvas.SetZIndex(rect, 10); // 10 is for tile, 11 - 19 is for middle layer objects and GameObjects have 20
        }

        #endregion
    }
}
