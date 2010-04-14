using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View;

namespace Sokoban {
    public interface INavigationController {
        void LoadView(string presenter, string view);
    }
}
