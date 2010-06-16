using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Sokoban
{
    /// <summary>
    /// Splash window
    /// </summary>
    public partial class Splash : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Splash()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show splash window (it's used at the start of the program)
        /// </summary>
        private static void createSplashWindow()
        {
            Splash sp = new Splash();
            sp.ShowDialog();
        }

        public static void Start()
        {
            Thread th = new Thread(new ThreadStart(Splash.createSplashWindow));
            th.Start();
            Thread.Sleep(1000);
            th.Abort();
            Thread.Sleep(1000);
        }
    }
}
