using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;
using Sokoban.Model.PluginInterface;
using System.Xml;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Model
{

    public partial class GameRepository
    {
        public event SetSizeDelegate DeskSizeChanged;
        public event GameObjectsLoadedDelegate GameObjectsLoaded;

        /// <summary>
        /// Function raises events when GameObject or RoundProperties are read from XML.
        /// Function expects valid (!) xml with scheme QuestScheme.
        /// </summary>
        /// <param name="xml"></param>
        public void LoadRoundFromXML(string xml)
        {                       
            // Initialization of fields
            gameObjects = new List<IGamePlugin>();
            movableElements = new List<IMovableElement>();
            fixedElements = new IGamePlugin[fieldsX, fieldsY];
            fixedTiles = new IGamePlugin[fieldsX, fieldsY];
            controllableByUserObjects = new List<IControllableByUserInput>();


            // Reading from the XML 
            XmlRoundReader xmlRounds = new XmlRoundReader(xml);
            xmlRounds.RoundPropertiesRead += new RoundPropertiesDelegate(OnLoadedRoundProperties);
            xmlRounds.GameObjectRead += new GameObjectPropertiesDelegate(OnLoadedGameObject);
            xmlRounds.LoadRoundSettings();

            // Fire events
            if (DeskSizeChanged != null)
            {
                DeskSizeChanged(fieldsX, fieldsY);
            }

            foreach (IGamePlugin gp in this.AllPlugins)
            {
                gp.Load(GameRepository.appPath);
            }

            // Load game variant settings
            if (this.gameVariant != null)
            {
                this.gameVariant.Load(GameRepository.appPath);
            }

            if (GameObjectsLoaded != null)
            {
                GameObjectsLoaded(gameObjects);
            }        
        }

        /*
         * 
         *  Private methods
         * 
         */

        private void OnLoadedGameObject(string pluginName, XmlNode node)
        {
            IGamePlugin gamePlugin = pluginService.RunPlugin(pluginName);

            // Plugin bootstrap
            gamePlugin.ProcessXmlInitialization(this.GameVariantName, this.fieldsX, this.fieldsY, node);
            

            // Find out plugin properties according to interfaces it implements
            if (this.integratePlugin(gamePlugin) == false)
            {
                throw new PluginLoadFailedException("Error in plugin `" + pluginName +"': Plugin must be either a tile or an object.");
            }
        }

        private void OnLoadedRoundProperties(string gameVariant, string roundName, int fieldsX, int fieldsY)
        {
            this.RoundName = roundName; // we want to notify
            this.GameVariantName = gameVariant;
            this.fieldsX = fieldsX;
            this.fieldsY = fieldsY;

            this.gameVariant = pluginService.RunVariant(gameVariant);
        }

        /// <summary>
        /// Method integrates plugin to the structures of GameRepository according to its properties (movable, fixed, ..)
        /// </summary>
        /// <param name="gamePlugin"></param>
        private bool integratePlugin(IGamePlugin gamePlugin)
        {
            // Plugin can receive keyboard input
            IControllableByUserInput controllableByUser = gamePlugin as IControllableByUserInput;

            if (controllableByUser != null)
            {
                controllableByUserObjects.Add(controllableByUser);

                if (controllableByUser.StepsCount != -1)
                {
                    stepsCountGameObject = controllableByUser;
                }
            }

            bool isElement = false;

            
            IFixedElement fixedElement = gamePlugin as IFixedElement;

            if (fixedElement != null)
            {
                isElement = true;
                fixedElements[fixedElement.PosX - 1, fixedElement.PosY - 1] = gamePlugin;
            }

            // Tiles
            IFixedTile fixedTile = gamePlugin as IFixedTile;

            if (fixedTile != null)
            {
                isElement = true;
                fixedTiles[fixedTile.PosX - 1, fixedTile.PosY - 1] = gamePlugin;
            }

            // Objects
            IMovableElement movableElement = gamePlugin as IMovableElement;

            if (movableElement != null)
            {
                isElement = true;
                movableElements.Add(movableElement);                
            }


            if (isElement)
            {
                gameObjects.Add(gamePlugin);
                gamePlugin.ID = gameObjects.Count - 1;
            }

            return isElement;
        }
    }
}
