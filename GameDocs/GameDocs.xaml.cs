using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AvalonDock;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sokoban.Model.GameDesk;
using Sokoban.View.GameDocsComponents;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;
using Sokoban.Model.Quests;
using Sokoban.Interfaces;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for GameDocs.xaml
    /// </summary>
    public partial class GameDocs : DocumentPane, INotifyPropertyChanged, ISolverProvider, ISolverPainter, IQuestHandler
    {
        public DockingManager dm;
        public ObservableCollection<DocumentContent> GameViews { get; set; }
        public GameDeskControl ActiveGameControl = null;
        private bool isKeyDown = false;
        private IUserInquirer userInquirer = null;
        private bool isSoundOn = true;
        private IGameServerCommunication gameServerCommunication = null;
        public event VoidBoolDelegate RestartAvaibilityChanged;

        public GameDocs()
        {
            InitializeComponent();            

            GameViews = new ObservableCollection<DocumentContent>();
        
            DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(
                GameManagerProperties.DockingManagerProperty,
                typeof(GameManagerProperties));

            prop.AddValueChanged(this, OnDockingManagerChanged);           
            GamesDocumentPane.SizeChanged += new SizeChangedEventHandler(GamesDocumentPane_SizeChanged);            
            this.DataContext = this;            
        }

        /// <summary>
        /// Shutdown all the games
        /// </summary>
        public void Terminate()
        {
            foreach (DocumentContent dc in GameViews)
            {
                GameDeskControl gameDeskControl = dc as GameDeskControl;
                if (gameDeskControl != null)
                {
                    gameDeskControl.Terminate();
                }
            }

            GameViews.Clear();
        }

        public void Initialize(IUserInquirer userInquirer, IGameServerCommunication gameServerCommunication)
        {
            this.userInquirer = userInquirer;
            this.gameServerCommunication = gameServerCommunication;
        }

        public void AddIntroduction()
        {
            Introduction doc = new Introduction();
            doc.Title = "Introduction";
            doc.InfoTip = doc.Title;
            doc.ContentTypeDescription = "";
            doc.Closing += new EventHandler<CancelEventArgs>(doc_Closing);
            doc.Closed += new EventHandler(doc_Closed);
            GameViews.Add(doc);             
        }

        private void OnDockingManagerChanged(object sender, EventArgs e)
        {
            dm = GameManagerProperties.GetDockingManager((DependencyObject)this);
            //dm.RequestDocumentClose += new EventHandler<RequestDocumentCloseEventArgs>(dm_RequestDocumentClose);                       

            //dm.SizeChanged += new SizeChangedEventHandler(dm_SizeChanged);
            dm.ActiveDocumentChanged += new EventHandler(dm_ActiveDocumentChanged);

            // make a new source
            Binding myBinding = new Binding("GameViews");
            myBinding.Source = this;
            dm.SetBinding(DockingManager.DocumentsSourceProperty, myBinding);
        }

        void dm_ActiveDocumentChanged(object sender, EventArgs e)
        {
            GameDeskControl oldVal = ActiveGameControl;

            if (dm.ActiveDocument != null)
            {
                GameDeskControl gdc = dm.ActiveDocument as GameDeskControl;

                if (gdc != null)
                {
                    ActiveGameControl = gdc;
                }
                else
                {
                    ActiveGameControl = null;
                }
            }
            else 
            {
                ActiveGameControl = null;                
            }

            restartAvaibilityChangeCheck(oldVal, ActiveGameControl);
        }

        private void restartAvaibilityChangeCheck(GameDeskControl oldValue, GameDeskControl newValue)
        {
            if (oldValue != newValue)
            {
                if (RestartAvaibilityChanged != null)
                {
                    RestartAvaibilityChanged(newValue != null);
                }
            }
        }

        public void SetActiveGameChanged(GameDeskControl g)
        {
            if (dm != null && dm.ActiveDocument != g)
            {
                dm.ActiveDocument = g;
            }

            restartAvaibilityChangeCheck(g, ActiveGameControl);
            ActiveGameControl = g;
        }

        void GamesDocumentPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dm_SizeChanged(sender, e);
        }

        void dm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActiveGameControl != null)
            {
                //double availableWidth = ((DocumentPane)(e.Source)).ActualWidth;
                double availableWidth = ((System.Windows.FrameworkElement)(ActiveGameControl.Content)).ActualWidth;
                double availableHeight = ((System.Windows.FrameworkElement)(ActiveGameControl.Content)).ActualHeight;

                foreach (GameDeskControl doc in GameViews)
                {
                    doc.Resize(availableWidth, availableHeight);
                }
            }
        }

        public void SetSoundsSettings(bool isEnabled)
        {
            this.isSoundOn = isEnabled;

            foreach (DocumentContent gdc in GameViews)
            {
                if (gdc is GameDeskControl)
                {
                    ((GameDeskControl)gdc).SetSounds(isEnabled);
                }
            }
        }

        public GameDeskControl Add(IQuest quest, GameMode gameMode)
        {
            if (GameViews.Count == 1)
            {
                if (GameViews[0] is Introduction)
                {
                    GameViews.RemoveAt(0);
                }
            }
            
            GameDeskControl doc = new GameDeskControl(quest, gameMode, userInquirer);
            doc.Title = quest.Name;
            doc.InfoTip = doc.Title;
            doc.ContentTypeDescription = "";
            doc.Closing += new EventHandler<CancelEventArgs>(doc_Closing);
            doc.Closed += new EventHandler(doc_Closed);
            doc.InfoPanel.SizeChanged += new SizeChangedEventHandler(doc.ResizeInfoPanel);
            doc.SetSounds(this.isSoundOn);
            GameViews.Add(doc);
            doc.Resize(GamesDocumentPane.ActualWidth, GamesDocumentPane.ActualHeight);

            //this.SelectedIndex = GameViews.Count - 1;
            

            this.SetActiveGameChanged(doc);

            return doc;                
        }

        private void doc_Closed(object sender, EventArgs e)
        {
            GameDeskControl oldVal = ActiveGameControl;

            if (sender is GameDeskControl)
            {
                GameDeskControl gdc = sender as GameDeskControl;

                if (gdc == ActiveGameControl)
                {
                    ActiveGameControl = null;                    
                }                
            }

            if (GameViews.Count == 0)
            {
                AddIntroduction();
            }

            restartAvaibilityChangeCheck(oldVal, ActiveGameControl);
        }

        private void doc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameDeskControl gdc = sender as GameDeskControl;

            if (gdc != null)
            {
                DebuggerIX.WriteLine(DebuggerTag.AppComponents, "[GameDocs]", "Close: " + string.Format("Tab {0} was closed", gdc.Title));
                gdc.Terminate();
                GameViews.Remove(gdc);
            }
        }

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            DebuggerIX.WriteLine(DebuggerTag.Keyboard, "KeyIsDown", "isKeyDown = " + isKeyDown.ToString());

            if (isKeyDown == false)
            {
                isKeyDown = true;
                if (ActiveGameControl != null)
                {
                    ActiveGameControl.KeyIsDown(sender, e);
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (ActiveGameControl != null)
            {
                ActiveGameControl.KeyIsUp(sender, e);                
            }

            isKeyDown = false;
            e.Handled = true;
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

        /// <summary>
        /// Returns width of current maze
        /// </summary>
        /// <returns></returns>
        public uint  GetMazeWidth()
        {
            if (ActiveGameControl == null)
            {
                throw new NoRoundIsOpenException();
            }
            else
            {
                return ActiveGameControl.GetMazeWidth();
            }
        }

        /// <summary>
        /// Returns height of current maze
        /// </summary>
        /// <returns></returns>
        public uint  GetMazeHeight()
        {
            if (ActiveGameControl == null)
            {
                throw new NoRoundIsOpenException();
            }
            else
            {
                return ActiveGameControl.GetMazeHeight();
            }
        }

        /// <summary>
        /// Returns string of XSB characters representing objects in maze; 
        /// may raise exception NotStandardSokobanVariantException when current maze is not standard
        /// </summary>
        /// <returns></returns>
        public string  SerializeMaze()
        {
            if (ActiveGameControl == null)
            {
                throw new NoRoundIsOpenException();
            }
            else
            {
                return ActiveGameControl.SerializeMaze();
            }
        }

        /// <summary>
        /// Not implemented!!
        /// </summary>
        public event GameObjectMovedDel SokobanMoved;

        public object GetIdentifier(SolverProviderIdentifierType spit)
        {
            if (ActiveGameControl == null)
            {
                throw new NoRoundIsOpenException();
            }
            else
            {
                return ActiveGameControl.GetIdentifier(spit);
            }
        }

        public string MovementsSoFar
        {
            get
            {
                if (ActiveGameControl == null)
                {
                    throw new NoRoundIsOpenException();
                }
                else
                {
                    return ActiveGameControl.MovementsSoFar;
                }
            }
        }


        #endregion

        #region ISolverPainter Members


        public void DrawSolverSolution(object mazeID, string solution, int sokobanX, int sokobanY)
        {
            GameDeskControl gdc = mazeID as GameDeskControl;

            // TODO MAY LEAD TO ILLEGAL STATE OF APP
            if (GameViews.Contains(gdc))
            {
                gdc.Game.DrawSolverSolution(solution, sokobanX, sokobanY);
            }
            else
            {
                MessageBoxShow("The solution cannot be displayed because the tab has been closed in the mean time.");                
            }
        }

        public void DrawSolverSolution(string solution, int sokobanX, int sokobanY)
        {
            ActiveGameControl.Game.DrawSolverSolution(solution, sokobanX, sokobanY);
        }

        #endregion

        #region IQuestHandler Members

        public IGameMatch QuestSelected(int leaguesID, int roundsID, IQuest quest, GameMode gameMode)
        {
            // TODO add params
            return this.Add(quest, gameMode);
        }

        #endregion

        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }        
    }
}
