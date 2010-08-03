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
using Sokoban.Lib;
using System.Globalization;
using System.Threading;

namespace Sokoban
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            Initialize();
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false); 
            DataContext = this;
        }

        private void Initialize()
        {
            PluginService.Initialize(@"D:\Bakalarka\Sokoban\Main\Plugins");
        }

        private void testList()
        {
            // Draw solution
            // gameManager.DrawSolverSolution("uuluuR", 3, 7);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.Start(DebuggerMode.File);
            loadQuest();            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DebuggerIX.Close();
        }

        private void loadQuest()
        {
            //string result = Sokoban.Properties.Resources.TestQuest;
            string result = Sokoban.Properties.Resources.SimpleQuest;
            //gameManager.Add(result);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                loadQuest();
                e.Handled = true;
            }
            else if (e.Key == Key.R)
            {
                gameManager.ActiveGameControl.RestartGame();
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            testList();
        }


    }
}