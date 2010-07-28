using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Solvers;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class GameDeskControl : ISolverProvider
    {
        public uint GetMazeWidth()
        {
            return this.game.GetMazeWidth();
        }

        public uint GetMazeHeight()
        {
            return game.GetMazeHeight();
        }

        public string SerializeMaze()
        {
            return game.SerializeMaze();
        }

        public event GameObjectMovedDel SokobanMoved
        {
            add
            {
                game.SokobanMoved += value;
            }
            remove
            {
                game.SokobanMoved -= value;
            }
        }

        public object GetIdentifier()
        {
            //return game.GetIdentifier();
            return this; // has to be GameDeskControl
        }        
    }
}
