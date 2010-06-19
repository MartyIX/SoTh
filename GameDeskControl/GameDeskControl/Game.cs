using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using AvalonDock;
using System.ComponentModel;
using Sokoban.View.GameDocsComponents;
using Sokoban.Lib;
using System.Windows.Media;
using System.Windows.Input;
using Sokoban.Lib.Events;
using System.Diagnostics;
using Sokoban.Model.PluginInterface;
using System.Xml;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model
{

    public class Game : IGame, IGameRealTime, ISolverProvider
    {
        // SETTINGS
        const int PHASE_CONST = 75;

        // API
        public string QuestValidationErrorMessage
        {
            get { return questValidationErrorMessage; }
        }

        // Private fields
        private GameDeskControl control; // assigned via RegisterVisual()
        private IQuest quest;
        private IGameRepository gameRepository;
        private Stopwatch stopwatch = new Stopwatch();
        private List<string> pluginSchemata = new List<string>();
        private string questValidationErrorMessage;
        

        private double phaseProp
        {
            get { return stopwatch.ElapsedMilliseconds / (double)PHASE_CONST; }
        }

        public Game(IQuest quest)
        {
            this.quest = quest;
            gameRepository = new GameRepository(); // loads also plugins

            gameRepository.GameRealTime = this;
            gameRepository.DeskSizeChanged += new SetSizeDelegate(gameRepository_DeskSizeChanged);
            gameRepository.GameObjectsLoaded += new GameObjectsLoadedDelegate(gameRepository_GameObjectsLoaded);
        }

        public bool IsQuestValid(IQuest quest)
        {
            bool isValid = true;

            var xml = new XmlDocument();
            xml.LoadXml(quest.WholeQuestXml);
            var names = new XmlNamespaceManager(xml.NameTable);
            names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");
            XmlNodeList nodes = xml.SelectNodes("/x:SokobanQuest/x:Round/x:GameObjects/*", names);
            
            XmlValidator xmlValidator = new XmlValidator();
            xmlValidator.AddSchema(null,  Sokoban.View.GameDocsComponents.Properties.Resources.QuestSchema); // main schema

            // We have to prevent to adding a schema twice to the collection of schemas (to xmlValidator)
            // Therefore we use Dictonary and then we extract each schema exactly once.
            Dictionary<string, string> schemas = new Dictionary<string, string>();

            // schemas from plugins
            foreach (XmlNode node in nodes)
            {                
                schemas[node.Name] = gameRepository.PluginService.GetPluginSchema(node.Name);
            }

            foreach (KeyValuePair<string, string> pair in schemas)
            {
                xmlValidator.AddSchema(null, pair.Value);
            }

            isValid = xmlValidator.IsValid(quest.WholeQuestXml);            
            questValidationErrorMessage = (isValid) ? "" : xmlValidator.GetErrorMessage();

            return isValid;
        }

        /// <summary>
        /// Register visuals from GameRepository
        /// </summary>
        /// <param name="gameObjects"></param>
        private void gameRepository_GameObjectsLoaded(List<IGamePlugin> gameObjects)
        {
            foreach (IGamePlugin g in gameObjects)
            {
                control.AddVisual(g.UIElement);
            }            
        }

        void gameRepository_DeskSizeChanged(int fieldsX, int fieldsY)
        {
            // We don't want a code that is dependent on graphics
            if (control != null)
            {
                control.SetSize(fieldsX, fieldsY);
            }
        }

        public void LoadCurrentRound()
        {
            loadRound(quest.ActualRoundXML);
        }

        private void loadRound(string roundXml)
        {
            bool roundLoaded = true;
            
            // may raise exception PluginLoadFailedException - it is catched in GameDocs.xaml.cs
            gameRepository.LoadRoundFromXML(roundXml); 

            if (roundLoaded)
            {
                this.startGame(); // For debugging
            }
        }


        private void startGame()
        {
            this.startRendering();
        }

        private void startRendering()
        {
            if (control != null)
            {
                CompositionTarget.Rendering += new EventHandler(OnRender);
                stopwatch.Start();
            }
        }

        private void stopRendering()
        {
            if (control != null)
            {
                CompositionTarget.Rendering -= OnRender;                
            }
        }


        private void OnRender(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds >= PHASE_CONST)
            {
                gameRepository.ProcessAllEvents();
                stopwatch.Reset();
                stopwatch.Start();
            }

            foreach (IGamePlugin g in gameRepository.GetGameObjects)
        	{               
                g.Draw(control.gamedeskCanvas, control.FieldSize, gameRepository.Time, phaseProp);
	        }
        }

        #region IGame Members

        public IQuest Quest {
            get { return this.quest; }
        }

        public double CurrentPhase
        {
            get { return this.phaseProp; }
        }

        public void RegisterVisual(GameDeskControl gameDeskControl)
        {
            this.control = gameDeskControl;
            this.control.OnResized += control_OnResized;
        }

        void control_OnResized(double width, double height, double fieldSize)
        {
            
        }

        #endregion

        public void MoveRequest(Key key)
        {
            gameRepository.MoveRequest(key);
        }

        public void StopMove(Key key)
        {
            gameRepository.StopMove(key);
        }

        public void Terminate()
        {
            this.stopRendering();
            gameRepository.Terminate();
        }

        #region ISolverProvider Members

        public uint GetMazeWidth()
        {
            return gameRepository.GetMazeWidth();
        }

        public uint GetMazeHeight()
        {
            return gameRepository.GetMazeHeight();
        }

        public string SerializeMaze()
        {
            return gameRepository.SerializeMaze();
        }

        #endregion
    }
}
