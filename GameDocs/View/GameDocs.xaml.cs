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
using AvalonDock;
using System.Diagnostics;
using Sokoban.View;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sokoban.Model.GameDesk;
using Sokoban.View.GameDocsComponents;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;

namespace Sokoban.View
{
    /// <summary>
    /// Interaction logic for GameDocs.xaml
    /// </summary>
    public partial class GameDocs : DocumentPane, INotifyPropertyChanged, ISolverProvider, ISolverPainter
    {
        public DockingManager dm;
        public ObservableCollection<DocumentContent> GameViews { get; set; }
        public GameDeskControl ActiveGameControl = null;

        private bool isKeyDown = false;


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
            foreach (GameDeskControl gameDeskControl in GameViews)
            {
                gameDeskControl.Terminate();
            }

            GameViews.Clear();
        }

        private void OnDockingManagerChanged(object sender, EventArgs e)
        {
            dm = GameManagerProperties.GetDockingManager((DependencyObject)this);
            dm.RequestDocumentClose += new EventHandler<RequestDocumentCloseEventArgs>(dm_RequestDocumentClose);
            dm.Loaded += new RoutedEventHandler(dm_Loaded);
            //dm.SizeChanged += new SizeChangedEventHandler(dm_SizeChanged);
            dm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(dm_PropertyChanged);

            // make a new source
            Binding myBinding = new Binding("GameViews");
            myBinding.Source = this;
            dm.SetBinding(DockingManager.DocumentsSourceProperty, myBinding);
        }

        void GamesDocumentPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dm_SizeChanged(sender, e);
        }

        void dm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocument" && dm.ActiveDocument != null)
            {
                if (dm.ActiveDocument != null && dm.ActiveDocument is GameDeskControl)
                {
                    SetActiveGameChanged((GameDeskControl)dm.ActiveDocument);
                }
            }
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

        void dm_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded(sender, e);
        }

        void dm_RequestDocumentClose(object sender, RequestDocumentCloseEventArgs e)
        {
            if (e.DocumentToClose is GameDeskControl)
            {
                GameDeskControl gdc = (GameDeskControl)e.DocumentToClose;
                DebuggerIX.WriteLine("[GameDocs]", "Close", string.Format("Tab {0} was closed", gdc.Title));
                gdc.Terminate();
                GameViews.Remove(gdc);
            }
        }

        public void Loaded(object sender, RoutedEventArgs e)
        {
            //string xmlLayout =
            //    "<DockingManager>" +
            //      "<ResizingPanel Orientation=\"Horizontal\">" +
            //      "  <DockablePane ResizeWidth=\"0.2125\" Anchor=\"Left\">" +
            //      "    <DockableContent Name=\"MyUserControl1\" AutoHide=\"false\" />" +
            //      "  </DockablePane>" +
            //      "  <DockablePane Anchor=\"Left\">" +
            //      "    <DockableContent Name=\"MyUserControl2\" AutoHide=\"false\" />" +
            //      "  </DockablePane>" +
            //      "</ResizingPanel>" +
            //      "<Hidden />" +
            //      "<Windows />" +
            //    "</DockingManager>";

            //StringReader sr = new StringReader(xmlLayout);
            //dockingManager.RestoreLayout(sr); 
            //Debug.WriteLine("DM Loaded");
        }

        public void SetActiveGameChanged(GameDeskControl g)
        {
            ActiveGameControl = g;
        }

        /// <summary>
        /// For testing purposes
        /// </summary>
        public void Add(string questXml)
        {
            Quest q = new Quest(questXml);
            this.Add(q);
        }

        public void Add(IQuest quest)
        {
            //try
            //{                

                GameDeskControl doc = new GameDeskControl(quest);
                doc.Title = quest.Name;
                doc.InfoTip = doc.Title;
                doc.ContentTypeDescription = "";
                doc.Closing += new EventHandler<CancelEventArgs>(doc_Closing);
                doc.Closed += new EventHandler(doc_Closed);
                doc.InfoPanel.SizeChanged += new SizeChangedEventHandler(doc.ResizeInfoPanel);
                GameViews.Add(doc);
                doc.Resize(GamesDocumentPane.ActualWidth, GamesDocumentPane.ActualHeight);
                
                this.SetActiveGameChanged(doc);
            //}
            /*
            catch (PluginLoadFailedException e)
            {
                MessageBox.Show("Cannot load quest. There's a problem in plugin: " + e.Message);
            }
            
            catch (NotValidQuestException e)
            {
                MessageBox.Show("Cannot load quest. There's a problem in quest XML: " + e.Message);
            }*/
        }

        private void doc_Closed(object sender, EventArgs e)
        {
            
        }

        private void doc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            
        }

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            DebuggerIX.WriteLine("[Keyboard]", "KeyIsDown; isKeyDown = " + isKeyDown.ToString());

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
            return ActiveGameControl.GetMazeWidth();
        }

        /// <summary>
        /// Returns height of current maze
        /// </summary>
        /// <returns></returns>
        public uint  GetMazeHeight()
        {
            return ActiveGameControl.GetMazeHeight();
        }

        /// <summary>
        /// Returns string of XSB characters representing objects in maze; 
        /// may raise exception NotStandardSokobanVariantException when current maze is not standard
        /// </summary>
        /// <returns></returns>
        public string  SerializeMaze()
        {
            return ActiveGameControl.SerializeMaze();
        }

        /// <summary>
        /// Not implemented!!
        /// </summary>
        public event GameObjectMovedDel SokobanMoved;

        public object GetIdentifier()
        {
            return ActiveGameControl.GetIdentifier();
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
                MessageBox.Show("The solution cannot be displayed because the tab has been closed in the mean time.");
            }
        }

        public void DrawSolverSolution(string solution, int sokobanX, int sokobanY)
        {
            ActiveGameControl.Game.DrawSolverSolution(solution, sokobanX, sokobanY);
        }

        #endregion
    }
}
