using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;

namespace Sokoban.Model
{
    public partial class GameRepository : IPluginParent
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

        public IGameRealTime GameRealTime
        {
            get { return this.game; }
            set { this.game = value; }
        }

        public PluginService PluginService
        {
            get { return pluginService; }
        }

        #region IPluginParent Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">one-based x-coord</param>
        /// <param name="y">one-based y-coord</param>
        /// <returns></returns>
        public IGamePlugin GetObstructionOnPosition(int x, int y)
        {
            IGamePlugin gp = null;

            // Fixed elements
                
            if (fixedElements[x - 1, y - 1] != null) 
            {
                gp = fixedElements[x - 1, y - 1];
            }
            else 
            {
                // Movable elements

                foreach (IGamePlugin g in this.movableElements)
                {
                    if (g.PosX == x && g.PosY == y)
                    {
                        gp = g;
                        break;
                    }
                }
            }

            return gp;
        }

        void IPluginParent.PropertyChanged(string property)
        {
            Notify(property);
        }

        public void Terminate()
        {
            if (pluginService != null)
            {
                pluginService.Terminate();
            }

            pluginService = null;
            
            gameIndicators = null;
            movableElements = null;
            fixedElements = null;
            controllableByUserObjects = null;
            gameObjects = null;
            DeskSizeChanged = null;
            GameObjectsLoaded = null;
        }

        #endregion
    }
}
