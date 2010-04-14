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
using System.Diagnostics;

namespace Sokoban
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Stopwatch stopwatch = new Stopwatch();
        bool isPressed = false;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isPressed == false)
            {
                Debug.WriteLine(e.Key.ToString() + " pressed");
                isPressed = true;
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.Key.ToString() + " released in " + stopwatch.ElapsedMilliseconds.ToString() + " miliseconds");
            isPressed = false;
        }


    }
}
