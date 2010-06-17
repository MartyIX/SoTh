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
using Sokoban.Solvers;

namespace SolversTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolverLibrary lib;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            lib = new SolverLibrary(@"D:\Bakalarka\Study\Solvers\SolverPlugins\YASS\FromSourceForge\YASS.dll", this);
            //lib = new SolverLibrary(@"D:\Bakalarka\Study\Solvers\SolverSDK\Plugins\SolverExample\Debug\SolverExample.dll", this);

            lib.GetConstraints();
            lib.GetPluginName();
            lib.ShowAbout();
            lib.Configure();

            lib.SolveEx(19, 17, "##############################################################   ################$  ################  $##############  $ $ ############# # ## ###########   # ## #####  ..## $  $          ..###### ### #@##  ..######     ########################################################################################################");

            lib.Unload();

            Close();
            */
        }
    }
}
