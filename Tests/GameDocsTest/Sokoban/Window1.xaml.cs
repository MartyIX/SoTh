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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Sokoban
{
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
            string result = Sokoban.Properties.Resources.TestQuest;
            gameManager.Add(result);          
        }
    }
}
