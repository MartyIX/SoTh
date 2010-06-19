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
using System.IO;
using System.Collections.ObjectModel;
using DummyObjectImplementations;
using Sokoban;

namespace SolversTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DummySolverProvider dsp;


        public MainWindow()
        {
            InitializeComponent();
			this.DataContext = this;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dsp = new DummySolverProvider(DummySolverProviderEnum.TestTwo);
            solversPane.Initialize(@"D:\Bakalarka\Sokoban\Main\Solvers\Solvers", (ISolverProvider)dsp, this);            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            solversPane.Terminate();   
        }
    }
}
