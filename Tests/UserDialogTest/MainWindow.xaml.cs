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
using Sokoban.View;
using Sokoban.Lib;

namespace UserDialogTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserDialog u = new UserDialog("Do you want to continue with next round?", new string[] { "Ok", "Cancel" }, null);
            u.Owner = this;
            u.Show();
            u.Completed += new VoidObjectStringStringDelegate(u_Completed);        
        }

        void u_Completed(object sender, string message, string answer)
        {
            MessageBox.Show(answer);
            Close();
        }
    }
}
