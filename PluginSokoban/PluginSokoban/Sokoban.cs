﻿using System;
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

namespace PluginSokoban
{
    public class Sokoban : MovableEssentials, IGamePlugin, IMovable, IControllableByUserInput
    {
        const int MAX_EVENTS_IN_KB = 3; 

        private EventType heldKeyEvent;
        private object syncRoot = new object();
        private bool moveRequestCancelled = true;

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

        public bool ProcessEvent(Int64 time, Event? e)
        {
            bool returnValue = false;
            Event ev = e.Value;

            switch (ev.what)
            {
                case EventType.goRight:
                case EventType.goLeft:
                case EventType.goUp:
                case EventType.goDown:

                    DebuggerIX.WriteLine("[PrepareMovement]", "Obj: " + ev.who.Description + "; Raised from EventID: " + ev.EventID.ToString());
                    PrepareMovement(ev.when, 0, (IGamePlugin)ev.who, ev.what);

                    this.MovementEventsInBuffer -= 1;
                    DebuggerIX.WriteLine("[GR-ProcessAllEvents]", ev.ToString());

                    returnValue = true;
                    break;

                case EventType.wentRight:
                case EventType.wentLeft:
                case EventType.wentUp:
                case EventType.wentDown:

                    #region wentXXX


                    if (heldKeyEvent != EventType.none && movementEventsInBuffer == 0 && timeMovementEnds <= time)
                    {
                        EventType newEvent = heldKeyEvent;
                        DebuggerIX.WriteLine("SokRepMvmt", "Raised from EventID = " + ev.EventID.ToString());
                        host.MakeImmediatePlan("SokRepMvmt", ev.who, newEvent);
                    }

                    returnValue = true;
                    break;

                    #endregion wentXXX
            }

            return returnValue;
        }

        public void Load()
        {
            // One-based values
            posX = 1;
            posY = 1;

            image = new System.Windows.Controls.Image();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,,/PluginSokoban;component/Resources/obj_S.png");
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

        #region IControllableByUserInput Members

        public void OnKeyDown(Key key, Int64 time, double phase)
        {
            DebuggerIX.WriteLine("[GR-MoveRequest]", ">>> MoveRequest <<<", "Time = " + time.ToString());

            EventType ev = EventTypeLib.ConvertFromKey(key);

            if (ev != EventType.none)
            {
                lock (syncRoot)
                {
                    this.heldKeyEvent = ev;

                    if (this.TimeMovementEnds <= time)
                    {
                        DebuggerIX.WriteLine("[GR-MoveRequest]", "Time is: " + time.ToString()
                            + "; phase = " + phase.ToString()
                            + "; Movement is not in progress.");

                        moveRequestCancelled = false;
                        // In this moment; events for @time are processed, therefore time + 1
                        //MakePlan("SokStartMov", time + 1, (GameObject)pSokoban, pSokoban.heldKeyEvent);
                        host.MakePlan("SokStartMov", time, (IGamePlugin)this, this.heldKeyEvent);

                        host.ProcessAllEvents(false, phase); // We don't want to update time
                    }
                    else if (this.MovementEventsInBuffer < MAX_EVENTS_IN_KB)
                    {
                        DebuggerIX.WriteLine("[GR-MoveRequest]", "MakePlan for move in time: " + (this.TimeMovementEnds).ToString());
                        host.MakePlan("SokKeyBuf", timeMovementEnds, (IGamePlugin)this, heldKeyEvent);
                    }
                }
            }
        }


        /// <summary>
        /// Don't continue with the movement of Sokoban
        /// </summary>
        public void OnKeyUp(Key key, Int64 time, double phase)
        {
            this.heldKeyEvent = EventType.none;
            moveRequestCancelled = true;
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

        bool ProcessXmlInitialization(XmlNode settings)
        {
            DebuggerIX.WriteLine("[Plugin]", this.Name, "ProcessXmlInitialization, settings: " + settings.InnerXml);

            return true;
        }

        #endregion
    }
}
