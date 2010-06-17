using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model;
using System.Collections;

namespace Sokoban.View
{   
    public interface IChooseConnectionView : IBaseView
    {
        string SelectedURL { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        void CloseWindow();
    }
}
