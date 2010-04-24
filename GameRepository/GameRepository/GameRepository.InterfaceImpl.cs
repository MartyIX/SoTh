using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;

namespace Sokoban.Model
{
    public partial class GameRepository
    {
        public int FieldsX
        {
            get { return fieldsX; }
        }

        public int FieldsY
        {
            get { return fieldsY; }
        }

        public IEnumerable<IGamePlugin> GetGameObjects
        {
            get { return gameObjects; }
        }

        public Int64 Time
        {
            get { return time; }
        }

        public GameRealTime Game
        {
            get { return this.game; }
            set { this.game = value; }
        }

        // LoadRoundFromXML is in file GameRepository.LoadRound.cs
    }
}
