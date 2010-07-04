using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Sokoban.Model.PluginInterface
{
    public interface IControllableByUserInput
    {
        bool OnKeyDown(Key key, Int64 time, double phase);
        bool OnKeyUp(Key key, Int64 time, double phase);
        int StepsCount { get; } // If the plugin doesn't want to provide the information; it has to return -1 always
    }
}
