using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model
{
    using System.Threading;
    using Sokoban.Model.PluginInterface;

    public partial class GameRepository : IBaseRepository
    {
        /// <summary>
        /// Processes given event
        /// </summary>
        /// <param name="ev">Event to process</param>
        public void ProcessEvent(Event? e)
        {
            Event ev = e.Value;


        }
    }
}
