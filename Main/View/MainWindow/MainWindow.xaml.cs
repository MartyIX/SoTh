using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sokoban.View;
using AvalonDock;
using System.Collections.ObjectModel;
using Sokoban;
using Sokoban.Model;
using Sokoban.View.ChooseConnection;
using System.Diagnostics;
using Sokoban.Lib;


namespace Sokoban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : System.Windows.Window
    {
        public ReadOnlyObservableCollection<string> MyProperty { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.Start(DebuggerMode.File);                        
            ApplicationRepository.Instance.OnStartUp();
            solversPane.Initialize(@"D:\Bakalarka\Sokoban\Main\Solvers\Solvers", this.gameManager, this);
            this.loadQuest();
        }        


        private void Window_Closed(object sender, EventArgs e)
        {
            DebuggerIX.Close();
        }

        private void loadQuest()
        {
            string result = Sokoban.Properties.Resources.TestQuest;
            gameManager.Add(result);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                loadQuest();
                e.Handled = true;
            }
            else
            {
                gameManager.ActiveGameControl.KeyIsDown(sender, e);
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            gameManager.ActiveGameControl.KeyIsUp(sender, e);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Correctly terminates everything in the main window that needs it
        /// </summary>
        private void Terminate()
        {
            if (gameManager != null)
            {
                gameManager.Terminate();
            }

            if (solversPane != null)
            {
                solversPane.Terminate(); // unload dynamic libraries
            }
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Terminate();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Terminate();
        }

        private void MenuItem_Console_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(consolePane);
        }

        private void MenuItem_Solvers_Click(object sender, RoutedEventArgs e)
        {
            SetVisibilityOfMenuItems(solversPane);
        }

        private void SetVisibilityOfMenuItems(DockableContent dc)
        {
            if (dc.Visibility == Visibility.Visible) // the value is set in ConvertBack of AvalonDockVisibilityConverter!!!
            {
                dockingManager.Show(dc);
            }
            else
            {
                dockingManager.Hide(dc);
            }
        }

        private void SetVisibilityOfDockableContents(DockableContent dc, DockableContentState state)
        {
            if (state == DockableContentState.Hidden)
            {
                dc.Visibility = Visibility.Hidden;
            }
        }

        private void solversPane_StateChanged(object sender, DockableContentState state)
        {
            SetVisibilityOfDockableContents(solversPane, state);
        }

        private void consolePane_StateChanged(object sender, DockableContentState state)
        {
            SetVisibilityOfDockableContents(consolePane, state);
        }


    }

    [ValueConversion(/* sourceType */ typeof(System.Windows.Visibility), /* targetType */ typeof(bool))]
    public class AvalonDockVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            Debug.Assert(targetType == typeof(bool));

            Visibility visibility = (Visibility)value;
            return visibility != Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;            
            return val ? Visibility.Visible : Visibility.Hidden;
        }
    }
}