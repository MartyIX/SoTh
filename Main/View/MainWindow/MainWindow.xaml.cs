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
            loadQuest();
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

    }
}