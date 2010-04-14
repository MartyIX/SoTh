using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model
{
    public partial class GameRepository : IBaseRepository
    {
        public int FieldsX
        {
            get { return fieldsX; }
        }

        public int FieldsY
        {
            get { return fieldsY; }
        }

        public IEnumerable<GameObject> GetGameObjects
        {
            get { return gameObjects; }
        }

        public Int64 Time
        {
            get { return time; }
        }

        public Game Game
        {
            get { return this.game; }
            set { this.game = value; }
        }

        // LoadRoundFromXML is in file GameRepository.LoadRound.cs
    }
}
