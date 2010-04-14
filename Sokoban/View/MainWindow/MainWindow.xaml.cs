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
            ApplicationRepository.Instance.OnStartUp();
        }        


        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            this.gameManager.Add();
        }


    }
}