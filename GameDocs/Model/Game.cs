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

namespace Sokoban.Model
{

    public class Game : IGame, IGameRealTime
    {
        // SETTINGS
        const int PHASE_CONST = 75;

        private GameDeskControl control; // assigned via RegisterVisual()
        private IQuest quest;
        private IGameRepository gameRepository;
        private Stopwatch stopwatch = new Stopwatch();

        private double phaseProp
        {
            get { return stopwatch.ElapsedMilliseconds / (double)PHASE_CONST; }
        }

        public Game(IQuest quest)
        {
            this.quest = quest;
            gameRepository = new GameRepository();
            gameRepository.GameRealTime = this;
            gameRepository.DeskSizeChanged += new SetSizeDelegate(gameRepository_DeskSizeChanged);
            gameRepository.GameObjectsLoaded += new GameObjectsLoadedDelegate(gameRepository_GameObjectsLoaded);
        }

        /// <summary>
        /// Register visuals from GameRepository
        /// </summary>
        /// <param name="gameObjects"></param>
        private void gameRepository_GameObjectsLoaded(List<GameObject> gameObjects)
        {
            foreach (GameObject g in gameObjects)
            {
                control.AddVisual(g.UI.Image);
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
            gameRepository.LoadRoundFromXML(roundXml);

            // For debugging
            this.startGame();
        }


        private void startGame()
        {
            this.initializeRendering();
        }

        private void initializeRendering()
        {
            if (control != null)
            {
                CompositionTarget.Rendering += new EventHandler(OnRender);
                stopwatch.Start();
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

            foreach (GameObject g in gameRepository.GetGameObjects)
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
            if (EventTypeLib.ConvertFromKey(key) != EventType.none)
            {
                gameRepository.MoveRequest(EventTypeLib.ConvertFromKey(key));
            }
        }

        public void StopMove()
        {
            gameRepository.StopMove();
        }


    }
}
