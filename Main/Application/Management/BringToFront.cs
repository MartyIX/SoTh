using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sokoban.Application
{
    public class ApplicationManagement
    {
        public static void BringToFront(Form form)
        {
            // Make this form the active form
            form.TopMost = true;
            form.Focus();
            form.BringToFront();
            form.TopMost = false;
        }
    }
}
