using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model;
using System.Collections;
using Sokoban.Lib;


namespace Sokoban
{
    public delegate void EmptyEventHandler();
    public delegate void KeyEventHandler(EventType e);
    public delegate void StringEventHandler(string str);
}