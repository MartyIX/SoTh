using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model;
using System.Collections;

namespace Sokoban.View
{   
    public interface ISettingsView : IBaseView
    {
        bool IsSplashEnabled { get; set; }
        void CloseWindow();
    }
}
