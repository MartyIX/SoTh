using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using AvalonDock;
using Sokoban.Lib;
using Sokoban.Model.GameDesk;
using Sokoban.WPF;
using Sokoban.Interfaces;

namespace Sokoban.View.GameDocsComponents
{

    /// <summary>
    /// Interaction logic for GameDeskControl.xaml
    /// </summary>
    public partial class GameDeskControl : DocumentContent, INotifyPropertyChanged
    {
        //
        // Public fields and properties
        //
        
        public event SizeChangedDelegate OnResized; // Fired in Resize(double, double)
        public bool IsChanged { get; set; }
        public static readonly RoutedCommand ViewCommand = new RoutedCommand();
        public int fieldsX = 8;
        public int fieldsY = 8;


        public bool DisplayBothDesks
        {
            get { return displayBothDesks; }
            set
            {
                displayBothDesks = value;
                this.Resize();
            }
        }

        // 
        // Private fields
        // 

        private double availableWidth = 200;
        private double availableHeight = 200;
        private Game game;
        private Game gameOpponent = null;

        private IGraphicsControl graphicsControl;
        private IGraphicsControl graphicsControlOpponent;

        private GameMode gameMode;
        private PlayingMode playingMode = PlayingMode.League;

        private NetworkModule networkModule = null;
        private IUserInquirer userInquirer = null;

        private double fieldSize = 25;
        private double time = 0;
        
        private IQuest quest = null;
        private bool displayBothDesks = false;

        //
        // Constructors
        //

        /// <summary>
        /// For Designer to display the control; do not use!
        /// </summary>
        public GameDeskControl()
        {
            constructorInitialization();
            //this.SizeChanged += new SizeChangedEventHandler(Resize);            
        }

        public GameDeskControl(IQuest quest, GameMode gameMode, IUserInquirer userInquirer)
        {           
            this.playingMode = PlayingMode.League;
            this.gameMode = gameMode;            
            this.quest = quest;
            this.userInquirer = userInquirer;
            constructorInitialization();

            this.SizeChanged += new SizeChangedEventHandler(Resize);

            // Game model for first player
            this.networkModule = new NetworkModule(this.userInquirer);
            game = new Game(quest, gameMode, GameDisplayType.FirstPlayer, this.networkModule, this.userInquirer);
            this.networkModule.GameChanged += new GameChangeDelegate(this.GameChangedHandler);
            this.networkModule.DisconnectRequest += new VoidObjectDelegate(networkModule_DisconnectRequest);            

            this.loadCurrentRound(game);

            if (gameMode == GameMode.TwoPlayers)
            {
                this.applyBlurEffect();  // removeBlurEffect is called in GameMatch.cs (SetNetworkConnection)           
                this.DisplayBothDesks = true;
                gameOpponent = new Game(quest, gameMode, GameDisplayType.SecondPlayer, this.networkModule, this.userInquirer);
                this.loadCurrentRound(gameOpponent);
            }

            this.networkModule.SetGameOpponent(gameOpponent);            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="roundID">One-based</param>
        public GameDeskControl(IQuest quest, GameMode gameMode, int roundID, IUserInquirer userInquirer)
        {
            playingMode = PlayingMode.Round;
            this.userInquirer = userInquirer;
            this.quest = quest;

            constructorInitialization();
         
            // Game model for first player

            quest.SetCurrentRound(roundID);
            game = new Game(quest, gameMode, GameDisplayType.FirstPlayer, null, this.userInquirer);
            this.loadCurrentRound(game);
        }


        private void constructorInitialization()
        {
            InitializeComponent();
            DataContext = this;

            graphicsControl = new GraphicsControl(this, gamedeskCanvas, gamedeskRect);
            graphicsControlOpponent = new GraphicsControl(this, gamedeskOpponentCanvas, gamedeskOpponentRect);

            if (!this.quest.IsLeague)
            {
                tbStaticLeagueName.Visibility = System.Windows.Visibility.Hidden;
                tbLeagueName.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                tbLeagueName.Text = this.quest.Name;
            }
        }

        //
        // Graphics components
        //
        
        public StackPanel InfoPanel
        {
            get { return infoPanel; }
        }

        public Canvas GamedeskCanvas
        {
            get { return gamedeskCanvas; }
        }

        public IGame Game
        {
            get { return game; }
        }

        public void SetSounds(bool isEnabled)
        {
            visualSoundsContainer.SetValue(Silencer.SilenceProperty, !isEnabled);
        }

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
            Resize(this.availableWidth, this.availableHeight - 25); // the 25 is just workaroud! TODO FIX
        }


        public string RoundName
        {
            get { return game.RoundName; }
        }

        public double FieldSize
        {
            get  { return fieldSize; }
            set 
            { 
                fieldSize = value;
                Notify("FieldSize"); /* in XAML is biding and ViewPort is changed automatically */
            }
        }

        public double Time
        {
            get { return time; }
            set { time = value; Notify("Time"); }
        }

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        
        public void DocumentContent_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.WriteLine(DebuggerTag.AppComponents, "[GameDeskControl]", "Loaded");
        }

        protected override void OnInitialized(EventArgs e)
        {
            DebuggerIX.WriteLine(DebuggerTag.AppComponents, "[GameDeskControl]", "Initialized");
            base.OnInitialized(e);
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

        


        //
        // Time updates
        //

        public void PauseTime()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;
            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounter'.");
            timeCounter.Pause();
        }

        private TimeSpan getTime()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;
            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounter'.");
            return timeCounter.Time;
        }

        private void timeStart()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;            
            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounter'.");
            timeCounter.Initialize(DateTime.Now);
            timeCounter.Start();
        }

        public void StopTime()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;

            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounter'.");
                
            timeCounter.Clear();
        }

    }

}
