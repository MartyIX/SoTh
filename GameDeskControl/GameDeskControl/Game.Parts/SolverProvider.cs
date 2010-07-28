using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Solvers;
using Sokoban.Lib;

namespace Sokoban.View.GameDocsComponents
{
    public partial class Game : ISolverProvider
    {
        #region ISolverProvider Members

        public uint GetMazeWidth()
        {
            return gameRepository.GetMazeWidth();
        }

        public uint GetMazeHeight()
        {
            return gameRepository.GetMazeHeight();
        }

        public string SerializeMaze()
        {
            return gameRepository.SerializeMaze();
        }

        public object GetIdentifier()
        {
            //return control;
            throw new NotImplementedException("Not implemented by purpose!");
        }

        public event GameObjectMovedDel SokobanMoved;


        #endregion

    }
}
