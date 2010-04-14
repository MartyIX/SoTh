#region usings
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Media;
using System.Net;
using System.Diagnostics;
using System.Xml;
using System;
#endregion usings

namespace Sokoban
{   
    public class LogEvent
    {
		#region Fields (6) 

        public bool ImportantEvent;
        public MovementDirection MovementDirection;
        public GameObject Obj;
        public int PosX;
        public int PosY;
        public string Time;

		#endregion Fields 

		#region Constructors (1) 

        public LogEvent(GameObject obj, int posX, int posY, MovementDirection movementDirection, string time, bool importantEvent)
        {
            Obj = obj;
            PosX = posX;
            PosY = posY;
            Time = time;
            MovementDirection = movementDirection;
            ImportantEvent = importantEvent;
        }

		#endregion Constructors 

		#region Methods (2) 

		// Public Methods (2) 

        public void AddAsChildren(XmlDocument xmlDoc, XmlNode parent)
        {
            XmlElement _objectID = xmlDoc.CreateElement("objectID");
            _objectID.InnerText = Obj.objectID.ToString();
            parent.AppendChild(_objectID);

            XmlElement _PosX = xmlDoc.CreateElement("PosX");
            _PosX.InnerText = PosX.ToString();
            parent.AppendChild(_PosX);

            XmlElement _PosY = xmlDoc.CreateElement("PosY");
            _PosY.InnerText = PosY.ToString();
            parent.AppendChild(_PosY);

            XmlElement _Time = xmlDoc.CreateElement("Time");
            _Time.InnerText = Time;
            parent.AppendChild(_Time);

            XmlElement _MovementDirection = xmlDoc.CreateElement("MovementDirection");
            _MovementDirection.InnerText = MovementDirection.ToString();
            parent.AppendChild(_MovementDirection);

            XmlElement _ImportantEvent = xmlDoc.CreateElement("ImportantEvent");
            _ImportantEvent.InnerText = (ImportantEvent) ? true.ToString() : false.ToString();
            parent.AppendChild(_ImportantEvent);

        }

        public override string ToString()
        {
            return "<objectID>" + Obj.objectID.ToString() + "</objectID>\n" +
                   "<PosX>" + PosX.ToString() + "</PosX>\n" +
                   "<PosY>" + PosY.ToString() + "</PosY>\n" +
                   "<Time>" + Time + "</Time>\n" +
                   "<MovementDirection>" + MovementDirection.ToString() + "</MovementDirection>\n";
        }

		#endregion Methods 
    }

    /// <summary>
    /// Class for logging actual round
    /// </summary>
    public class LogList
    {
		#region Fields (7) 

        // Private
        private List<LogEvent> events = new List<LogEvent>();
        private GameDeskView form;
        // Events with sokoban and mosters (movements of boxes is not important as step in logging for user)
        private List<int> importantEvents = new List<int>();
        private List<LogEvent> initialPositions = new List<LogEvent>();
        // Public
        /// <summary>
        /// Variable sets if game ended with death of Sokoban
        /// </summary>
        public bool isLastFrameIsDeath = false;
        /// <summary>
        /// What "frame" was displayed as the last one last time
        /// </summary>
        public int LastFrame = 0;
        private Player player;
        private bool isEnabledAddingEvents = true;

        public bool IsEnabledAddingEvents
        {
            get { return isEnabledAddingEvents; }
            set { isEnabledAddingEvents = value; }
        }


		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="form">Reference to main form</param>
        public LogList(GameDeskView form, Player player)
        {
            this.player = player;
            this.form = form;            
        }

		#endregion Constructors 

		#region Methods (7) 

		// Public Methods (7) 

        /// <summary>
        /// Function logs an event that happened on game desk.
        /// </summary>
        /// <param name="obj">Which GameObject</param>
        /// <param name="posX">x-coordinate on game desk</param>
        /// <param name="posY">y-coordinate on game desk</param>
        /// <param name="time">Time when event happened</param>
        /// <param name="direction">Direction of object at the time of event</param>
        /// <param name="importantEvent">If event should be displayed in listbox lbLog</param>
        public void AddEvent(GameObject obj, int posX, int posY, string time, MovementDirection direction, bool importantEvent)
        {
            if (isEnabledAddingEvents == true)
            {
                events.Add(new LogEvent(obj, posX, posY, direction, time, importantEvent));

                if (importantEvent)
                {
                    importantEvents.Add(events.Count - 1);
                    form.lbLog.Items.Add(form.lTime.Text + " | " + posX.ToString() + ", " + posY.ToString());
                }
            }
        }

        /// <summary>
        /// Function deletes all information that were logged. Prepare the class for logging of a new round.
        /// </summary>
        public void Clear()
        {
            events.Clear();
            initialPositions.Clear();
            importantEvents.Clear();
            form.lbLog.Items.Clear();
            isLastFrameIsDeath = false;
            LastFrame = 0;
        }

        /// <summary>
        /// Function loads a .srg file with information about moves of all object and adjust main form to display the moves.
        /// </summary>
        /// <param name="filename">Absolute path to the file with saved game</param>
        public void LoadFromFile(string filename)
        {
            this.Clear();
            player.StopGame();

            form.EnableReplayMode();

            XmlDocument xmlDoc = new XmlDocument();
            XmlTextReader xmlTextReader = new XmlTextReader(filename);
            xmlDoc.Load(xmlTextReader);

            form.actLeagueName = "-- Replay --";
            form.actLeagueXml = xmlDoc.InnerXml;
            player.round = 1;

            player.InitializeRound();

            SetInitPositions(true);
            player.LostGame();

            XmlNodeList recordXML = xmlDoc.GetElementsByTagName("Record");
            XmlNodeList eventsInXML = xmlDoc.GetElementsByTagName("event");

            for (int i = 1; i < eventsInXML.Count; i++) // note the trick that we skip first event which is known from round definition
            {
                GameObject obj = (GameObject)player.gameDesk.gameObjects[int.Parse(eventsInXML[i]["objectID"].InnerText)];

                this.AddEvent(obj, int.Parse(eventsInXML[i]["PosX"].InnerText),
                                   int.Parse(eventsInXML[i]["PosY"].InnerText),
                                   eventsInXML[i]["Time"].InnerText,
                                   (MovementDirection)Enum.Parse(typeof(MovementDirection), eventsInXML[i]["MovementDirection"].InnerText),
                                   (eventsInXML[i]["ImportantEvent"].InnerText == "True") ? true : false);
            }

            isLastFrameIsDeath = (recordXML[0]["isLastFrameDeath"].InnerText == "True") ? true : false;
            form.ShowLoggingWindow(false, false);
        }

        /// <summary>
        /// Function sets GameObjects to the position of step @endMov.
        /// </summary>
        /// <param name="fromFrame">Which frame was last displayed</param>
        /// <param name="endMov">Which frame should be displayed</param>
        /// <param name="time">Return time of endMov frame via reference</param>
        public void MoveUpToEvent(int fromFrame, int endMov, out string time)
        {
            if (!(endMov < importantEvents.Count && endMov >= 0) || !(fromFrame < importantEvents.Count && fromFrame >= 0))
            {
                time = "NA";
                return;
            }

            endMov = importantEvents[endMov];
            fromFrame = importantEvents[fromFrame];
            time = events[endMov].Time;

            if (fromFrame > endMov)
            {
                fromFrame = 0;
                SetInitPositions(false);
            }

            for (int i = fromFrame; i < endMov + 1; i++)
            {
                events[i].Obj.posX = events[i].PosX;
                events[i].Obj.posY = events[i].PosY;
                events[i].Obj.direction = events[i].MovementDirection;
            }

            for (int i = 0; i < initialPositions.Count; i++)
            {
                initialPositions[i].Obj.PozX = initialPositions[i].Obj.posX;
                initialPositions[i].Obj.PozY = initialPositions[i].Obj.posY;

                if (initialPositions[i].Obj == player.gameDesk.pSokoban)
                {
                    // we displays last frame and it's death of Sokoban
                    if (isLastFrameIsDeath && importantEvents[importantEvents.Count - 1] == endMov)
                    {
                        player.gameDesk.pSokoban.ShowDeath();
                    }
                    else
                    {
                        //form.SokobanSetOrientation(initialPositions[i].Obj.direction);
                    }
                }

            }
        }

        /// <summary>
        /// Function saves initial position of a object.
        /// </summary>
        /// <param name="obj">a GameObject</param>
        /// <param name="posX">x-coordinate on game desk (1 to maxX)</param>
        /// <param name="posY">y-coordinate on game desk (1 to maxY)</param>
        /// <param name="movementDirection">the direction the object is turned to</param>
        public void SaveInitPositions(GameObject obj, int posX, int posY, MovementDirection movementDirection)
        {
            initialPositions.Add(new LogEvent(obj, posX, posY, movementDirection, "0:00", false));
        }

        /// <summary>
        /// Function saves game to the file for later replay. It's theoreticaly possible to save game before game ended.
        /// </summary>
        /// <param name="filename">Absolute path to the file where game should be saved</param>
        public void SaveToFile(string filename)
        {
            // remove other rounds
            XmlDocument xmlDoc = new XmlDocument(); // create an xml document object
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(form.actLeagueXml));
            xmlDoc.Load(xmlTextReader);

            XmlNodeList rounds = xmlDoc.GetElementsByTagName("Round");
            XmlNode parent = rounds[0].ParentNode;

            for (int i = 0; i < rounds.Count; i++)
            {
                if (i + 1 != player.round) parent.RemoveChild(rounds[i]);
            }

            StringBuilder sb = new StringBuilder();

            //Create a new node.
            XmlElement record = xmlDoc.CreateElement("Record");

            //Add the node to the document.
            parent.AppendChild(record);

            for (int i = 0; i < events.Count; i++)
            {
                XmlElement tmp = xmlDoc.CreateElement("event");
                events[i].AddAsChildren(xmlDoc, tmp);
                record.AppendChild(tmp);
            }

            XmlElement isLastFrameDeathXML = xmlDoc.CreateElement("isLastFrameDeath");
            isLastFrameDeathXML.InnerText = isLastFrameIsDeath.ToString();
            record.AppendChild(isLastFrameDeathXML);

            StringBuilder sbXML = new StringBuilder();
            xmlDoc.Save(filename);

            MessageBox.Show("Game was saved.");
        }

        /// <summary>
        /// Function sets all game objects on initial positions.
        /// </summary>
        /// <param name="moveOnDesk">if the change is even on game desk</param>
        public void SetInitPositions(bool moveOnDesk)
        {
            foreach (var initPos in initialPositions)
            {
                // uses setter of properties PozX and PozY of GameObject
                if (moveOnDesk)
                {
                    initPos.Obj.PozX = initPos.PosX; // with special setter
                    initPos.Obj.PozY = initPos.PosY;
                }
                else
                {
                    initPos.Obj.posX = initPos.PosX; // initPos.Obj.pozX is ordinary variable
                    initPos.Obj.posY = initPos.PosY;
                }

                initPos.Obj.direction = initPos.MovementDirection;
            }
        }

		#endregion Methods 
    }
}