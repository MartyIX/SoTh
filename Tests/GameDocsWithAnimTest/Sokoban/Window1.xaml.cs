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
using Sokoban;
using Sokoban.Model;
using AvalonDock;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Sokoban.Lib;

namespace Sokoban
{
    using Debugger = Sokoban.Lib.Debugger;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();            
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Debugger.Start(DebuggerMode.File);
            //string result = Sokoban.Properties.Resources.TestQuest;
            string result = Sokoban.Properties.Resources.SokobanQuest;
            gameManager.Add(result);          
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            gameManager.KeyIsDown(sender, e);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                Debugger.WriteLine("[DebugMark]", "--------------------");
            }
            else
            {
                gameManager.KeyIsUp(sender, e);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debugger.Close();
        }
    }
}
