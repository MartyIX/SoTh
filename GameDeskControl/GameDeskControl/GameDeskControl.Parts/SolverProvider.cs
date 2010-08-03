using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Solvers;
using Sokoban.Lib;
using Sokoban.Lib.Exceptions;

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

        public object GetIdentifier(SolverProviderIdentifierType spit)
        {
            //return game.GetIdentifier();

            if (spit == SolverProviderIdentifierType.DocumentPaneInstance)
            {
                return this; // has to be GameDeskControl
            }
            else if (spit == SolverProviderIdentifierType.RoundsID)
            {
                if (game == null || game.GameRepository == null)
                {
                    throw new NoRoundIsOpenException();
                }                
                else if (game.GameRepository.GameVariantName == "Ordinary")
                {
                    return game.Quest.CurrentRoundID;
                }
                else
                {
                    throw new NotStandardSokobanVariantException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string MovementsSoFar
        {
            get { return game.MovementsSoFar; }
        }
    }
}
