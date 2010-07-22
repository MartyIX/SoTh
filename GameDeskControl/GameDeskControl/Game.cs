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
using Sokoban.Solvers;
using Sokoban.View;
using System.Windows;
using System.Windows.Data;

namespace Sokoban.Model
{

    public class Game : IGame, IGameRealTime, ISolverProvider, INotifyPropertyChanged, ISolverPainter
    {
        // SETTINGS
        const int PHASE_CONST = 30;

        // API
        public string QuestValidationErrorMessage
        {
            get { return questValidationErrorMessage; }
        }

        public string RoundName
        {
            get { return gameRepository.RoundName; }
        }

        public IGameRepository GameRepository {
            get { return gameRepository; }
        }

        // Private fields
        private IGraphicsControl control; // assigned via RegisterVisual()
        private IQuest quest;
        private IGameRepository gameRepository;
        private Stopwatch stopwatch = new Stopwatch();
        private List<string> pluginSchemata = new List<string>();
        private string questValidationErrorMessage;
        private GameStatus gameStatus = GameStatus.Unstarted;
        private GameDisplayType gameDisplayType;
        private bool isRendering = false;

        public Game(IQuest quest, GameDisplayType gameDisplayType)
        {
            this.quest = quest;
            this.gameDisplayType = gameDisplayType;
        }

        private double phaseProp
        {
            get { return stopwatch.ElapsedMilliseconds / (double)PHASE_CONST; }
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
            if (control == null) throw new InvalidStateException("RegisterVisual has to be called prior to loading of a round.");

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
                control.SetGameDeskSize(fieldsX, fieldsY);
            }
        }

        public void LoadCurrentRound()
        {
            loadRound(quest.ActualRoundXML);
        }

        public event VoidChangeDelegate PreRoundLoaded;

        private void loadRound(string roundXml)
        {
            this.removeOldPainter();
            this.StopRendering();
            this.clearControlContent();

            if (gameRepository != null) gameRepository.Terminate();

            solversSokobanRef = null;
            gameRepository = new GameRepository(); // loads also plugins

            gameRepository.GameRealTime = this;
            gameRepository.DeskSizeChanged += new SetSizeDelegate(gameRepository_DeskSizeChanged);
            gameRepository.GameObjectsLoaded += new GameObjectsLoadedDelegate(gameRepository_GameObjectsLoaded);
            gameRepository.GameStarted += new VoidChangeDelegate(startGame);
        
            if (PreRoundLoaded != null) PreRoundLoaded();

            // may raise exception PluginLoadFailedException - it is catched in GameDocs.xaml.cs
            gameRepository.LoadRoundFromXML(roundXml);

            StartRendering();
        }

        private void clearControlContent()
        {
            if (control != null)
            {
                control.ClearVisuals();
            }
        }

        /// <summary>
        /// Called when from gameRepository as event (GameRepository.GameStarted)
        /// </summary>
        private void startGame()
        {
            gameStatus = GameStatus.Running;
            this.StartRendering();            
        }

        /// <summary>
        /// Called from startGame()
        /// </summary>
        public void StartRendering()
        {
            if (control != null)
            {
                if (isRendering == false)
                {
                    CompositionTarget.Rendering += new EventHandler(OnRender);
                    stopwatch.Start();
                    isRendering = true;
                }
            }
        }

        /// <summary>
        /// Called from this.Terminate() and when loading a new round (i.e. this.loadRound(string xml))
        /// </summary>
        public void StopRendering()
        {
            if (control != null)
            {
                if (isRendering == true)
                {
                    CompositionTarget.Rendering -= OnRender;
                    isRendering = false;
                }
            }
        }

        //
        // Rendering
        //
        private void OnRender(object sender, EventArgs e)
        {
            if (gameStatus == GameStatus.Running)
            {
                if (stopwatch.ElapsedMilliseconds >= PHASE_CONST)
                {
                    gameRepository.ProcessAllEvents();
                    stopwatch.Reset();
                    stopwatch.Start();
                }

            }

            drawGamePlugins();

            if (solverPainter != null)
            {
                solverPainter.Redraw(control.FieldSize);
            }
        }

        #region IGame Members

        private void drawGamePlugins()
        {
            foreach (IGamePlugin g in gameRepository.GetGameObjects)
            {
                if (phaseProp < 1)
                {
                    g.Draw(control.Canvas, control.FieldSize, gameRepository.Time, phaseProp);
                }
                else
                {
                    g.Draw(control.Canvas, control.FieldSize, gameRepository.Time, 0);
                }
            }
        }

        public IQuest Quest {
            get { return this.quest; }
        }

        public double CurrentPhase
        {
            get { return this.phaseProp; }
        }

        public void RegisterVisual(IGraphicsControl graphicsControl)
        {
            this.control = graphicsControl;
        }

        #endregion

        public bool MoveRequest(Key key)
        {
            // Start game by pressing a keyboard key
            return gameRepository.MoveRequest(key, this.phaseProp);
        }

        public bool StopMove(Key key)
        {
            return gameRepository.StopMove(key, this.phaseProp);
        }

        public void Terminate()
        {
            this.StopRendering();
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

        public object GetIdentifier()
        {
            //return control;
            throw new NotImplementedException("Not implemented by purpose!");
        }

        public event GameObjectMovedDel SokobanMoved;


        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion

        #region ISolverPainter Members

        private SolverPainter solverPainter;

        private IMovableElement solversSokobanRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solution">Characters: l,r,u,d,L,R,U,D in row. 
        ///    Uppercase letters means moving of a box. Format is used as result of a solver)</param>
        /// <param name="sokobanX">One-based x-coordinate of Sokoban</param>
        /// <param name="sokobanY">One-based y-coordinate of Sokoban</param>
        public void DrawSolverSolution(string solution, int sokobanX, int sokobanY)
        {
            if (solversSokobanRef == null)
            {

                foreach (IGamePlugin gp in this.GameRepository.GetGameObjects)
                {
                    if (gp.Name == "Sokoban")
                    {
                        IMovableElement me = gp as IMovableElement;

                        // Register event only when it is needed; when to unregister??
                        if (SokobanMoved == null)
                        {
                            if (me != null)
                            {
                                me.ElementMoved += new GameObjectMovedDel(sokoban_ElementMoved);
                            }
                        }

                        solversSokobanRef = me;

                        break;
                    }
                }

                if (solversSokobanRef == null)
                {
                    MessageBox.Show("Sokoban object was not found in this game variant.");
                }
            }

            // only for game variants with sokoban
            if (solversSokobanRef != null)
            {
                if (solversSokobanRef.PosX != sokobanX || solversSokobanRef.PosY != sokobanY)
                {
                    MessageBox.Show("Sokoban changed position during the time the solver was finding a solution.\nThe solution is therefore invalid.");
                }
                else
                {
                    // Remove old painter
                    this.removeOldPainter();

                    // New painter
                    solverPainter = new SolverPainter(control.Canvas, sokobanX, sokobanY, control.FieldSize, solution);
                    solverPainter.Draw();
                    SokobanMoved += solverPainter.Update;
                }
            }
        }

        private void removeOldPainter()
        {
            if (solverPainter != null)
            {
                SokobanMoved -= solverPainter.Update;
                solverPainter.Terminate();
            }
        }

        public void DrawSolverSolution(object mazeID, string solution, int sokobanX, int sokobanY)
        {
            if (mazeID != this)
            {
                throw new Exception("Wrong reference to maze encountered.");
            }

            DrawSolverSolution(solution, sokobanX, sokobanY);
        }

        void sokoban_ElementMoved(int newX, int newY, char direction)
        {
            if (SokobanMoved != null)
            {
                SokobanMoved(newX, newY, direction);
            }
        }

        #endregion

    }
}
