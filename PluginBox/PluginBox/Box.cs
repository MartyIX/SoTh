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

namespace PluginBox
{
    public partial class Box : MovableEssentials, IGamePlugin, IMovableElement
    {
        private object syncRoot = new object();

        public Box(IPluginParent host) : base(host)
        {
        }


        #region IPlugin Members

        public string Name
        {
            get { return "Box"; }
        }

        public string Description
        {
            get { return "Oficial Box implementation"; }
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
            return base.ProcessEvent(time, ev);
        }

        public void Load()
        {
            // One-based values
            posX = 2;
            posY = 2;
            obstructionLevel = 5;
            Speed = 0;

            image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/PluginBox;component/Resources/obj_B.png");
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
            get { return PluginBox.Properties.Resources.XmlSchema; }
        }


        public bool ProcessXmlInitialization(int mazeWidth, int mazeHeight, XmlNode settings)
        {
            this.fieldsX = mazeWidth;
            this.fieldsY = mazeHeight;

            DebuggerIX.WriteLine("[Plugin]", this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);

            return true;
        }

    }
}
