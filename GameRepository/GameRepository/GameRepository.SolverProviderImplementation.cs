using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using System.Timers;
using Sokoban.Lib;
using System.Threading;
using Sokoban.Lib.Events;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;

namespace Sokoban.Model
{
    public partial class GameRepository : ISolverProvider
    {

        #region ISolverProvider Members

        public uint GetMazeWidth()
        {
            return (uint)fieldsX;
        }

        public uint GetMazeHeight()
        {
            return (uint)fieldsY;
        }


        /// <summary>
        /// Returns maze in XSB format
        /// 
        /// @ - sokoban
        /// + - sokoban on target
        /// # - wall
        /// $ - box
        /// . - target
        /// * - box on target
        ///   - nothing
        /// </summary>
        /// <returns></returns>
        public string SerializeMaze()
        {
            StringBuilder sb = new StringBuilder(fieldsX * fieldsY);
            sb.Length = fieldsX * fieldsY;

            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = ' ';
            }

            int posInStr = 0;

            foreach (IGamePlugin go in gameObjects)
            {
                posInStr = go.PosX - 1 + (go.PosY - 1) * FieldsX;

                if (go.Name == "Sokoban")
                {
                    if (sb[posInStr] == ' ') // nothing
                    {
                        sb[posInStr] = '@'; // Sokoban
                    }
                    else if (sb[posInStr] == '.') // Target
                    {
                        sb[posInStr] = '+'; // Sokoban on target
                    }
                    else
                    {
                        throw new NotStandardSokobanVariantException("Unexpected Sokoban on position: " +
                            go.PosX.ToString() + "x" + go.PosY.ToString());
                    }
                }
                else if (go.Name == "Box")
                {
                    if (sb[posInStr] == ' ') // nothing
                    {
                        sb[posInStr] = '$'; // Box
                    }
                    else if (sb[posInStr] == '.') // Target
                    {
                        sb[posInStr] = '*'; // Box on target
                    }
                    else
                    {
                        throw new NotStandardSokobanVariantException("Unexpected Box on position: " +
                            go.PosX.ToString() + "x" + go.PosY.ToString());
                    }
                }
                else if (go.Name == "Aim") // target
                {
                    if (sb[posInStr] == ' ') // nothing
                    {
                        sb[posInStr] = '.'; // target
                    }
                    else if (sb[posInStr] == '@') // Sokoban
                    {
                        sb[posInStr] = '+'; // Sokoban on target
                    }
                    else if (sb[posInStr] == '$') // Box
                    {
                        sb[posInStr] = '*'; // Box on target
                    }
                    else
                    {
                        throw new NotStandardSokobanVariantException("Unexpected Sokoban on position: " +
                            go.PosX.ToString() + "x" + go.PosY.ToString());
                    }
                }
                else if (go.Name == "Wall")
                {
                    if (sb[posInStr] == ' ') // nothing
                    {
                        sb[posInStr] = '#'; // Wall
                    }
                    else
                    {
                        throw new NotStandardSokobanVariantException("Unexpected Wall on position: " +
                            go.PosX.ToString() + "x" + go.PosY.ToString());
                    }
                }
                else
                {
                    throw new NotStandardSokobanVariantException("Unexpected object '" + go.Name + "' on position: " +
                            go.PosX.ToString() + "x" + go.PosY.ToString());
                }
            } 


            return sb.ToString();
        }

        /// <summary>
        /// Not implemented!!
        /// </summary>
        public event GameObjectMovedDel SokobanMoved;

        public object GetIdentifier()
        {
            throw new NotImplementedException("This method is not implemented by purpose.");
        }

        #endregion
    }
}
