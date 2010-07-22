using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using System.Windows.Input;
using Sokoban.Lib;
using Sokoban.Lib.Events;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.IO;
using System.Reflection;

namespace PluginSokoban
{
    public partial class Sokoban : MovableEssentials, IGamePlugin, IMovableElement, IControllableByUserInput, INotifyPropertyChanged
    {
        const int MAX_EVENTS_IN_KB = 2; 

        private EventType heldKeyEvent;
        private object syncRoot = new object();
        private bool moveRequestCancelled = true;
        private MediaElement sounds;
        private DateTime lastTimeHitWallPlayed = DateTime.Now;

        public Sokoban(IPluginParent host) : base(host)
        {            
        }


        #region IPlugin Members

        public string Name
        {
            get { return "Sokoban"; }
        }

        public string Description
        {
            get { return "Oficial Sokoban implementation"; }
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
            bool returnValue = false;

            switch (ev.what)
            {
                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    base.SetOrientation(ev.what);

                    DebuggerIX.WriteLine("[Sokoban]", "ProcessEvent", ev.what.ToString() + "; Raised from EventID: " + ev.EventID.ToString());
                    DebuggerIX.WriteLine("[Sokoban]", "", ev.ToString());
                    returnValue = base.ProcessEvent(time, ev);                    
         
                    break;

                case EventType.wentRight:
                case EventType.wentLeft:
                case EventType.wentUp:
                case EventType.wentDown:

                    #region wentXXX
                   
                    movementEventsInBuffer--;
                    DebuggerIX.WriteLine("SokKeyBuf", "Key buffer: " + movementEventsInBuffer.ToString() +
                        " / " + MAX_EVENTS_IN_KB.ToString());

                    if (heldKeyEvent != EventType.none && movementEventsInBuffer == 0 && timeWholeMovementEnds <= time)
                    {
                        EventType newEvent = heldKeyEvent;
                        DebuggerIX.WriteLine("SokRepMvmt", "Raised from EventID = " + ev.EventID.ToString());
                        base.MakePlan("SokRepMvmt", ev.when, ev.who, newEvent);
                    }

                    returnValue = true;
                    break;

                    #endregion wentXXX

                case EventType.hitToTheWall:

                    return processHitToWall();

                    break;

                default:
                    returnValue = base.ProcessEvent(time, ev);                    
                    break;
            }

            return returnValue;
        }

        protected bool processHitToWall()
        {
            // we don't want to play more sounds at one time
            if (sounds.NaturalDuration.HasTimeSpan == true)
            {
                if (DateTime.Now.Subtract(lastTimeHitWallPlayed).TotalMilliseconds > sounds.NaturalDuration.TimeSpan.TotalMilliseconds)
                {
                    double length = sounds.NaturalDuration.TimeSpan.TotalMilliseconds;
                    sounds.Play();
                    sounds.Position = TimeSpan.Zero;
                    lastTimeHitWallPlayed = DateTime.Now;
                }
            }
            else
            {
                sounds.Play();
                sounds.Position = TimeSpan.Zero;
            }


            return true;
        }

        public void Load()
        {
            // One-based values
            posX = 1;
            posY = 1;
            ObstructionLevel = 10;
            StepsCount = 0; // we want to notify

            //
            // Textures
            //
            
            image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/PluginSokoban;component/Resources/obj_S.png");
            bi.EndInit();
            image.Source = bi;

            this.uiElement = image;

            //
            // Sounds
            //

            sounds = new MediaElement();
            sounds.LoadedBehavior = MediaState.Manual; // TODO ADD TO THE INSTALLER
            sounds.Source = new Uri(@"D:\Bakalarka\Sokoban\Main\bin\Debug\Plugins\Sounds\HitToTheWall.wav");
            host.RegisterMediaElement(sounds);
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

        #region IControllableByUserInput Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <param name="phase"></param>
        /// <returns>Returns if the key was handled</returns>
        public bool OnKeyDown(Key key, Int64 time, double phase)
        {
            DebuggerIX.WriteLine("[GR-MoveRequest]", ">>> MoveRequest <<<", "Time = " + time.ToString() + 
                "; Phase = " + phase.ToString() + "; Key = " + key.ToString());

            EventType ev = EventTypeLib.ConvertFromKey(key);

            if (ev != EventType.none)
            {
                lock (syncRoot)
                {
                    this.heldKeyEvent = ev;

                    if (this.MovementEventsInBuffer < MAX_EVENTS_IN_KB)
                    {
                        if (this.TimeWholeMovementEnds <= time)
                        {
                            DebuggerIX.WriteLine("[GR-MoveRequest]", "Movement is not in progress. Movement starts at time: " +
                                time.ToString());

                            moveRequestCancelled = false;
                            // In this moment; events for @time are processed, therefore time + 1
                            //MakePlan("SokStartMov", time + 1, (GameObject)pSokoban, pSokoban.heldKeyEvent);

                            base.MakePlan("SokStartMov", time, (IGamePlugin)this, this.heldKeyEvent);

                            host.ProcessAllEvents(false, phase); // We don't want to update time
                        }
                        else 
                        {                        
                            base.MakePlan("SokKeyBuf", timeWholeMovementEnds, (IGamePlugin)this, heldKeyEvent);
                            DebuggerIX.WriteLine("[GR-MoveRequest]", "Request is buffered. Movement start at time: " + (this.TimeWholeMovementEnds).ToString());
                        }
                    }
                    else
                    {
                        DebuggerIX.WriteLine("[GR-MoveRequest]", "Ignored; Buffer is full: " + this.MovementEventsInBuffer.ToString());
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Don't continue with the movement of Sokoban
        /// </summary>
        public bool OnKeyUp(Key key, Int64 time, double phase)
        {
            this.heldKeyEvent = EventType.none;
            moveRequestCancelled = true;

            return EventTypeLib.ConvertFromKey(key) != EventType.none;
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
            get { return PluginSokoban.Properties.Resources.XmlSchema; }
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

            this.Speed = 6;
            this.fieldsX = mazeWidth;
            this.fieldsY = mazeHeight;

            DebuggerIX.WriteLine("[Plugin]", this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);

            return true;
        }

        #endregion

    }
}
