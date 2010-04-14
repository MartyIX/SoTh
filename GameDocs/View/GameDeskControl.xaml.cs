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
using Sokoban.Model;
using Sokoban.Model.GameDesk;
using Sokoban.Lib;
using System.Diagnostics;

namespace Sokoban.View.GameDocsComponents
{
    using Debugger = Sokoban.Lib.Debugger;

    /// <summary>
    /// Interaction logic for GameDeskControl.xaml
    /// </summary>
    public partial class GameDeskControl : DocumentContent, INotifyPropertyChanged
    {        
        private double availableWidth = 0;
        private double availableHeight = 0;
        private Game game;

        public event SizeChangedDelegate OnResized; // Fired in Resize(double, double)

        /// <summary>
        /// For Designer to display the control; do not use!
        /// </summary>
        public GameDeskControl()
        {
            InitializeComponent();
            DataContext = this;
            this.SizeChanged += new SizeChangedEventHandler(Resize);
        }

        public GameDeskControl(IQuest quest)
        {
            InitializeComponent();
            DataContext = this;
            this.SizeChanged += new SizeChangedEventHandler(Resize);

            // Game model
            game = new Game(quest);
            game.RegisterVisual(this);
            game.LoadCurrentRound();
        }

        private int fieldsX;
        private int fieldsY;        
        private string roundName = "";

        public string RoundName
        {
            get { return roundName; }
            set { roundName = value; Notify("RoundName"); }
        }

        private double fieldSize = 50;

        public double FieldSize
        {
            get { return fieldSize; }
            set { fieldSize = value; 
                Notify("FieldSize"); /* in XAML is biding and ViewPort is changed automatically */}
        }

        private double steps = 0;

        public double Steps
        {
            get { return steps; }
            set { steps = value; Notify("Steps"); }
        }

        private double time = 0;

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
            double availableWidth = this.availableWidth;
            double availableHeight = this.availableHeight;

            Resize(availableWidth, availableHeight);
        }

        public void AddVisual(UIElement c)
        {
            this.gamedeskCanvas.Children.Add(c);
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

            // Compute sizes of gamedesk
            availableWidth = availableWidth - infoPanel.ActualWidth - 10 /* margin */;
            availableWidth = (availableWidth < 0) ? 0 : availableWidth;
            availableHeight = (availableHeight < 0) ? 0 : availableHeight;
        
            double fieldX = Math.Floor(availableWidth / (double)fieldsX);
            double fieldY = Math.Floor(availableHeight / (double)fieldsY);
            FieldSize = Math.Min(fieldX, fieldY); // We want to notify

            double width = FieldSize * fieldsX;
            double height = FieldSize * fieldsY;
                
            this.gamedeskRect.Width = width;
            this.gamedeskRect.Height = height;

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
                this.availableWidth = ((DocumentPane)(e.Source)).ActualWidth;
                this.availableHeight = ((System.Windows.FrameworkElement)(this.Content)).ActualHeight;

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

        /// <summary>
        /// Changes control appearance
        /// </summary>
        /// <param name="fieldsX"></param>
        /// <param name="fieldsY"></param>
        public void SetSize(int fieldsX, int fieldsY)
        {
            this.fieldsX = fieldsX;
            this.fieldsY = fieldsY;

            Resize();
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

        public bool IsChanged { get; set; }

        public static readonly RoutedCommand ViewCommand = new RoutedCommand();

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        public void DocumentContent_Loaded(object sender, RoutedEventArgs e)
        {
            Debugger.WriteLine("[GameDeskControl]", "Loaded");
        }

        protected override void OnInitialized(EventArgs e)
        {
            Debugger.WriteLine("[GameDeskControl]", "Initialized");
            base.OnInitialized(e);
        }

        /// <summary>
        /// Capturing keys in order to move Sokoban
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            game.MoveRequest(e.Key);            
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            game.StopMove();            
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
