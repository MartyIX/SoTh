using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public partial class MainWindow
    {
        private void MessageBoxShow(string message)
        {
            if (userInquirer != null)
            {
                userInquirer.ShowMessage(message);
            }
        }
    }
}
