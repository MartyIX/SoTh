using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Sokoban.Model.PluginInterface
{
    public interface IControllableByUserInput
    {
        void OnKeyDown(Key key);
        void OnKeyUp(Key key);
    }
}
