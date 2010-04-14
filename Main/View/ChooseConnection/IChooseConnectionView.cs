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
        string SelectedURL { get; }
        string Username { get; }
        string Password { get; }
        void CloseWindow();
    }
}
