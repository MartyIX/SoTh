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
using Sokoban.Lib.Exceptions;

namespace PluginSokoban
{
    public partial class Sokoban : MovableEssentials, IGamePlugin, IMovableElement, IControllableByUserInput, INotifyPropertyChanged
    {
        const int MAX_EVENTS_IN_KB = 2; 

        private EventType heldKeyEvent;
        private object syncRoot = new object();
        private MediaElement sounds;
        private DateTime lastTimeHitWallPlayed = DateTime.Now;

        public Sokoban(IPluginParent host) : base(host)
        {
            Initialize(this, false);
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
            get { return "2.00"; }
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


                    if (!host.IsSimulationActive) return true;
                
                    base.SetOrientation(ev.what);

                    DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[Sokoban] ProcessEvent", ev.what.ToString() + "; Raised from EventID: " + ev.EventID.ToString());
                    DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "", "[Sokoban]" + ev.ToString());
                    
                    
                    IGamePlugin obstruction = base.ProcessGoEvent(time, ev);

                    if (obstruction != null && obstruction.Name == "Box")
                    {
                        this.host.GameVariant.CheckRound(time, "BoxMoved", null);
                    }

                    returnValue = true;

                    break;

                case EventType.wentRight:
                case EventType.wentLeft:
                case EventType.wentUp:
                case EventType.wentDown:

                    #region wentXXX

                    base.ProcessEvent(time, ev);
                    
                    DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[Sokoban]", "Key buffer: " + movementEventsInBuffer.ToString() +
                        " / " + MAX_EVENTS_IN_KB.ToString());

                    if (heldKeyEvent != EventType.none && movementEventsInBuffer == 0 && timeWholeMovementEnds <= time)
                    {
                        EventType newEvent = heldKeyEvent;
                        DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[Sokoban]", "Raised from EventID = " + ev.EventID.ToString());
                        base.MakePlan("SokRepMvmt", ev.when, ev.who, newEvent);
                    }

                    returnValue = true;
                    break;

                    #endregion wentXXX

                case EventType.hitToTheWall:

                    return processHitToWall();

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
                    sounds.Position = TimeSpan.Zero;
                    sounds.Play();                    
                    lastTimeHitWallPlayed = DateTime.Now;
                }
            }
            else
            {
                sounds.Position = TimeSpan.Zero;
                sounds.Play();                
            }


            return true;
        }

        public void Load(string appPath)
        {
            // One-based values
            obstructionLevel = -1; // special value!
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

            if (!File.Exists(appPath + @"\Plugins\Sounds\HitToTheWall.wav"))
            {
                throw new PluginLoadFailedException("File `" + appPath + @"\Plugins\Sounds\HitToTheWall.wav" + "` could not be found.");
            }
            else
            {
                sounds = new MediaElement();
                sounds.LoadedBehavior = MediaState.Manual; // TODO ADD TO THE INSTALLER
                sounds.Source = new Uri(appPath + @"\Plugins\Sounds\HitToTheWall.wav");
                host.RegisterMediaElement(sounds);
            }
        }

        public void Unload()
        {
            image = null;
            uiElement = null;
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
            DebuggerIX.WriteLine(DebuggerTag.Keyboard, "[GR-MoveRequest]", ">>> MoveRequest <<< Time = " + time.ToString() + 
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
                            DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[GR-MoveRequest]", 
                                "Movement is not in progress. Movement starts at time: " + time.ToString());

                            // In this moment; events for @time are processed, therefore time + 1
                            //MakePlan("SokStartMov", time + 1, (GameObject)pSokoban, pSokoban.heldKeyEvent);

                            base.MakePlan("SokStartMov", time, (IGamePlugin)this, this.heldKeyEvent);

                            host.ProcessAllEvents(false, phase); // We don't want to update time
                        }
                        else 
                        {                        
                            base.MakePlan("SokKeyBuf", timeWholeMovementEnds, (IGamePlugin)this, heldKeyEvent);
                            DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[GR-MoveRequest]", 
                                "Request is buffered. Movement start at time: " + (this.TimeWholeMovementEnds).ToString());
                        }
                    }
                    else
                    {
                        DebuggerIX.WriteLine(DebuggerTag.SimulationNotableEvents, "[GR-MoveRequest]", 
                            "Ignored; Buffer is full: " + this.MovementEventsInBuffer.ToString());
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

        public void MessageReceived(string messageType, object message, IGamePlugin p)
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

            DebuggerIX.WriteLine(DebuggerTag.Plugins, this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            posX = int.Parse(settings["PosX"].InnerText);
            posY = int.Parse(settings["PosY"].InnerText);

            return true;
        }

        #endregion

        public override int ObstructionLevel(IGamePlugin asker)
        {
            if (asker.Name == "Monster")
            {
                return -1;
            }
            else
            {
                return 10;
            }
        }


    }
}
