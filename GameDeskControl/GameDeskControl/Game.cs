using System.Collections.Generic;
using Sokoban.Model.GameDesk;
using System.ComponentModel;
using Sokoban.View.GameDocsComponents;
using Sokoban.Lib;
using System.Windows.Input;
using System.Diagnostics;
using Sokoban.Lib.Exceptions;
using Sokoban.Interfaces;
using Sokoban.Model;

namespace Sokoban.View.GameDocsComponents
{

    public partial class Game : IGame, IGameRealTime, INotifyPropertyChanged
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

        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set { gameStatus = value; }
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
        private GameMode gameMode;
        private INetworkService networkService;
        private IUserInquirer userInquirer = null;

        public Game(IQuest quest, GameMode gameMode, GameDisplayType gameDisplayType, INetworkService networkService, IUserInquirer userInquirer)
        {
            this.quest = quest;
            this.gameDisplayType = gameDisplayType;
            this.gameMode = gameMode;
            this.networkService = networkService;
            this.userInquirer = userInquirer;

            if (this.quest == null) throw new NotValidQuestException("Quest is not valid");
        }

        private double phaseProp
        {
            get { return stopwatch.ElapsedMilliseconds / (double)PHASE_CONST; }
        }        
        
        public IQuest Quest {
            get { return this.quest; }
        }

        public double CurrentPhase
        {
            get { return this.phaseProp; }
        }

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
            gameStatus = Lib.GameStatus.Unstarted;
            this.StopRendering();
            this.removeOldPainter();

            // nulling events
            PreRoundLoaded = null;
            SokobanMoved = null;
            
            gameRepository.Terminate();
        }

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
    }
}
