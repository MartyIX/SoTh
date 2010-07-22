using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using AvalonDock;
using System.Diagnostics;
using Sokoban.Lib;
using Sokoban.Model;
using Sokoban.Model.GameDesk;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;
using Sokoban.WPF;
using Sokoban.Interfaces;
using Sokoban.Networking;

namespace Sokoban.View.GameDocsComponents
{

    /// <summary>
    /// Interaction logic for GameDeskControl.xaml
    /// </summary>
    public partial class GameDeskControl : DocumentContent, INotifyPropertyChanged, ISolverProvider, IGameMatch
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
        private double fieldSize = 25;
        private double time = 0;
        private PlayingMode playingMode = PlayingMode.League;
        private IQuest quest = null;
        private bool displayBothDesks = false;
        private IConnection networkConnection = null;

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

        public GameDeskControl(IQuest quest, GameMode gameMode)
        {
            constructorInitialization();

            this.playingMode = PlayingMode.League;
            this.gameMode = gameMode;
            this.SizeChanged += new SizeChangedEventHandler(Resize);
            this.quest = quest;

            // Game model for first player
            game = new Game(quest, GameDisplayType.FirstPlayer);
            this.loadCurrentRound(game);

            if (gameMode == GameMode.TwoPlayers)
            {
                this.DisplayBothDesks = true;
                gameOpponent = new Game(quest, GameDisplayType.SecondPlayer);
                this.loadCurrentRound(gameOpponent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="roundID">One-based</param>
        public GameDeskControl(IQuest quest, GameMode gameMode, int roundID)
        {
            constructorInitialization();
            playingMode = PlayingMode.Round;

            // Game model for first player

            quest.SetCurrentRound(roundID);
            game = new Game(quest, GameDisplayType.FirstPlayer);
            this.loadCurrentRound(game);
        }


        private void constructorInitialization()
        {
            InitializeComponent();
            DataContext = this;

            graphicsControl = new GraphicsControl(this, gamedeskCanvas, gamedeskRect);
            graphicsControlOpponent = new GraphicsControl(this, gamedeskOpponentCanvas, gamedeskOpponentRect);
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


        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
            Resize(this.availableWidth, this.availableHeight - 25); // the 25 is just workaroud! TODO FIX
        }

        public void Reload()
        {
            Terminate();
            loadCurrentRound(game);
        }

        private void loadCurrentRound(Game game)
        {
            visualSoundsContainer.Children.Clear(); // remove all sounds
            
            if (game == this.game)
            {
                game.RegisterVisual(this.graphicsControl);
                game.PreRoundLoaded += new VoidChangeDelegate(game_PreRoundLoaded);
            }
            else
            {
                game.RegisterVisual(this.graphicsControlOpponent);
            }
            
            game.LoadCurrentRound();

            if (!game.IsQuestValid(this.quest))
            {
                DebuggerIX.WriteLine("[Game]", "Quest", "Quest is not valid!");
                throw new NotValidQuestException(game.QuestValidationErrorMessage);
            }

            if (game == this.game)
            {
                game.GameRepository.GameStarted += new VoidChangeDelegate(timeStart);
                DataContext = this;
                tbSteps.DataContext = this.Game.GameRepository;
                Notify("GameRepository");
                game.StartRendering();
            }
            else
            {
                game.StartRendering();
            }
        }

        /// <summary>
        /// Called after the GameRepository instance is created but before round and plugins are loaded
        /// </summary>
        void game_PreRoundLoaded()
        {
            game.GameRepository.MediaElementAdded += new NewMediaElementDelegate(GameRepository_MediaElementAdded);
        }

        private void GameRepository_MediaElementAdded(MediaElement me)
        {
            visualSoundsContainer.Children.Add(me);
        }

        public void Terminate()
        {
            this.timeStop();
            game.Terminate();            
            if (gameOpponent != null) gameOpponent.Terminate();
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

        /// <summary>
        /// Resize according to GameDeskControl cache
        /// </summary>
        public void Resize()
        {
            double availableWidth = this.availableWidth - this.BorderThickness.Left - this.BorderThickness.Right;
            double availableHeight = this.availableHeight - this.BorderThickness.Bottom - this.BorderThickness.Top;

            Resize(availableWidth, availableHeight);
        }

        /// <summary>
        /// Available width/height for whole GameDeskControl!!
        /// </summary>
        /// <param name="availableWidth"></param>
        /// <param name="availableHeight"></param>
        public void Resize(double availableWidth, double availableHeight)
        {
            // Cache the sizes
            this.availableWidth = availableWidth;
            this.availableHeight = availableHeight;

            double width = 0;
            double height = 0;

            if (this.displayBothDesks == false)
            {
                this.gamedeskOpponentCanvas.Visibility = System.Windows.Visibility.Collapsed;
                
                // Compute sizes of gamedesk
                availableWidth = availableWidth - infoPanel.ActualWidth /* margin */;
                availableWidth = (availableWidth < 0) ? 0 : availableWidth;
                availableHeight = (availableHeight < 0) ? 0 : availableHeight;

                double fieldX = Math.Floor(availableWidth / (double)fieldsX);
                double fieldY = Math.Floor(availableHeight / (double)fieldsY);
                FieldSize = Math.Min(fieldX, fieldY); // We want to notify

                width = FieldSize * fieldsX;
                height = FieldSize * fieldsY;

                this.gamedeskRect.Width = width;
                this.gamedeskRect.Height = height;
            }
            else
            {
                this.gamedeskOpponentCanvas.Visibility = System.Windows.Visibility.Visible;

                // Compute sizes of gamedesk; 10 = margin; dividing by two is because we need place for two desks
                availableWidth = (availableWidth - infoPanel.ActualWidth) / 2; 
                availableWidth = (availableWidth < 0) ? 0 : availableWidth;
                availableHeight = (availableHeight < 0) ? 0 : availableHeight;

                double fieldX = Math.Floor(availableWidth / (double)fieldsX);
                double fieldY = Math.Floor(availableHeight / (double)fieldsY);
                FieldSize = Math.Min(fieldX, fieldY); // We want to notify

                width = FieldSize * fieldsX;
                height = FieldSize * fieldsY;

                this.gamedeskRect.Width = width;
                this.gamedeskRect.Height = height;

                this.gamedeskOpponentRect.Width = width;
                this.gamedeskOpponentRect.Height = height;

                /*this.gamedeskOpponentCanvas.Height = height;
                this.gamedeskCanvas.Height = height;*/
            }

            if (OnResized != null)
            {
                OnResized(width, height, FieldSize);
            }
        }

        /// <summary>
        /// Handler for event SizeChanged of GamesDocumentPane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Resize(object sender, SizeChangedEventArgs e)
        {
            if (e.Source is DocumentPane)
            {
                this.availableWidth = e.NewSize.Width;
                this.availableHeight = e.NewSize.Height;
                Resize(availableWidth, availableHeight);
            }            
        }

        /// <summary>
        /// Handler for event SizeChanged of InfoPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResizeInfoPanel(object sender, SizeChangedEventArgs e)
        {
            // Resize: Data from cache but InfoPanel sizes are now correct!
            Resize(this.availableWidth, this.availableHeight);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void OnCanClose(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.Handled = true;
            //e.CanExecute = false;
        }
        
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        public void DocumentContent_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.WriteLine("[GameDeskControl]", "Loaded");
        }

        protected override void OnInitialized(EventArgs e)
        {
            DebuggerIX.WriteLine("[GameDeskControl]", "Initialized");
            base.OnInitialized(e);
        }

        /// <summary>
        /// Capturing keys in order to move Sokoban
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameMode == GameMode.SinglePlayer || (gameMode == GameMode.TwoPlayers && networkConnection != null))
            {
                e.Handled = game.MoveRequest(e.Key);
            }
            else
            {
                MessageBox.Show("Connection has not been established yet. Please wait.");
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (gameMode == GameMode.SinglePlayer || (gameMode == GameMode.TwoPlayers && networkConnection != null))
            {
                e.Handled = game.StopMove(e.Key);
            }
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

        #region ISolverProvider Members

        public uint GetMazeWidth()
        {
            return this.game.GetMazeWidth();
        }

        public uint GetMazeHeight()
        {
            return game.GetMazeHeight();
        }

        public string SerializeMaze()
        {
            return game.SerializeMaze();
        }

        public event GameObjectMovedDel SokobanMoved
        {   
            add
            {
                game.SokobanMoved += value;
            }
            remove
            {
                game.SokobanMoved -= value;
            }
        }

        public object GetIdentifier()
        {
            //return game.GetIdentifier();
            return this; // has to be GameDeskControl
        }

        #endregion


        //
        // Time updates
        //

        private void timeStart()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;            
            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounte'.");
            timeCounter.Initialize(DateTime.Now);
            timeCounter.Start();
        }

        private void timeStop()
        {
            BindableTimeCounter timeCounter = this.Resources["timeCounter"] as BindableTimeCounter;

            if (timeCounter == null) throw new Exception("Cannot found resource `timeCounter'.");
                
            timeCounter.Clear();
        }

        #region IGameMatch Members

        public void SetNetworkConnection(IConnection connection)
        {
            this.networkConnection = connection;
        }

        #endregion
    }



    [ValueConversion(/* sourceType */ typeof(int), /* targetType */ typeof(Rect))]
    public class FieldSizeToRectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Rect));

            double fieldSize = double.Parse(value.ToString());
            return new Rect(0, 0, 2 * fieldSize, 2 * fieldSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // should not be called in our example
            throw new NotImplementedException();
        }
    }
}
