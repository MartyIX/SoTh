using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib.Exceptions;
using Sokoban.Model;
using System.Windows.Media;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game
    {
        //
        // Events
        //
        public event VoidChangeDelegate PreRoundLoaded;
        
        

        public bool IsQuestValid(IQuest quest)
        {
            bool isValid = true;

            var xml = new XmlDocument();
            xml.LoadXml(quest.WholeQuestXml);
            var names = new XmlNamespaceManager(xml.NameTable);
            names.AddNamespace("x", "http://www.martinvseticka.eu/SoTh");
            XmlNodeList nodes = xml.SelectNodes("/x:SokobanQuest/x:Round/x:GameObjects/*", names);

            XmlValidator xmlValidator = new XmlValidator();
            xmlValidator.AddSchema(null, Sokoban.View.GameDocsComponents.Properties.Resources.QuestSchema); // main schema

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

        

        private void loadRound(string roundXml)
        {
            this.removeOldPainter();
            this.StopRendering();
            this.clearControlContent();

            if (gameRepository != null) gameRepository.Terminate();

            solversSokobanRef = null;
            gameRepository = new GameRepository(gameMode, gameDisplayType, this); // loads also plugins

            gameRepository.GameRealTime = this;
            gameRepository.DeskSizeChanged += new SetSizeDelegate(gameRepository_DeskSizeChanged);
            gameRepository.GameObjectsLoaded += new GameObjectsLoadedDelegate(gameRepository_GameObjectsLoaded);

            if (gameDisplayType == GameDisplayType.FirstPlayer)
            {
                gameRepository.GameStarted += new VoidChangeDelegate(StartGame);
                gameRepository.GameChanged += new GameChangeDelegate(gameRepository_GameChanged);
            }

            if (PreRoundLoaded != null) PreRoundLoaded();

            // may raise exception PluginLoadFailedException - it is catched in GameDocs.xaml.cs
            gameRepository.LoadRoundFromXML(roundXml);

            StartRendering();
        }

        void gameRepository_GameChanged(GameChange gameChange)
        {
            if (gameMode == GameMode.SinglePlayer)
            {
                if (gameChange == GameChange.Won)
                {
                    if (quest.RoundsNo > 1)
                    {
                        if (!quest.IsLast())
                        {
                            ShowQuestion(inquirySinglePlayerPlayNextRoundOfLeague, new string[] { "Yes", "No" });
                        }
                        else
                        {
                            MessageBoxShow("Congratulations! You've won the round and completed the league!");
                        }
                    }
                    else
                    {
                        MessageBoxShow("Congratulations! You've won the round!");
                    }

                    this.gameStatus = GameStatus.Finished;
                    control.StopTime();
                }
                else if (gameChange == GameChange.StopCountingTime)
                {
                    control.PauseTime();
                }
                else if (gameChange == GameChange.Lost)
                {                    
                    this.gameStatus = GameStatus.Finished;
                    control.PauseTime();

                    this.ShowQuestion(inquiryRestart, new string[] { "Yes", "No" }); // answer is processed in UserInquirer.cs                    
                }
            }
            else
            {
                throw new NotImplementedException("not implemented");
            }
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
        public void StartGame()
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

                    if (gameMode == GameMode.TwoPlayers && gameDisplayType == GameDisplayType.FirstPlayer)
                    {
                        ProcessNetworkTraffic(gameRepository.Time);
                    }
                }

            }

            drawGamePlugins();

            if (solverPainter != null)
            {
                solverPainter.Redraw(control.FieldSize);
            }
        }

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

        public void RegisterVisual(IGraphicsControl graphicsControl)
        {
            this.control = graphicsControl;
        }       
    }
}
