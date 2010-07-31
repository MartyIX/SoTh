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
using Sokoban.Lib;
using AvalonDock;
using System.Diagnostics;

namespace Sokoban.View.GameDocsComponents
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : DocumentContent
    {
        public Introduction()
        {
            InitializeComponent();
        }

        public void DocumentContent_Loaded(object sender, RoutedEventArgs e)
        {
            DebuggerIX.WriteLine(DebuggerTag.AppComponents, "[GameDeskControl]", "Loaded");
        }

        private void link1_Click(object sender, RoutedEventArgs e)
        {
            string navigateUri = link1.NavigateUri.ToString();
            openLink(navigateUri);
            e.Handled = true;
        }

        private void link2_Click(object sender, RoutedEventArgs e)
        {
            string navigateUri = link2.NavigateUri.ToString();
            openLink(navigateUri);
            e.Handled = true;
        }

        private void openLink(string navigateUri)
        {
            Process.Start(new ProcessStartInfo(navigateUri));
        }
   
    }
}
