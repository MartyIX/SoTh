﻿using System;
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

    public partial class GameRepository : IBaseRepository
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
            movableElements = new List<IGamePlugin>();
            fixedElements = new IGamePlugin[fieldsX, fieldsY];
            fixedTiles = new IGamePlugin[fieldsX, fieldsY];
            controllableByUserObjects = new List<IControllableByUserInput>();
            gameIndicators = new List<IGamePlugin>();


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

            if (GameObjectsLoaded != null)
            {
                GameObjectsLoaded(gameObjects);
            }

            gameState = GameState.Running;
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
            //try
            //{
            gamePlugin.Load();
            gamePlugin.ProcessXmlInitialization(this.fieldsX, this.fieldsY, node);
            //}
            //catch (Exception e)
            //{
            //   throw new PluginLoadFailedException("Plugin " + pluginName + " failed to load. Error: " + e.Message);
            //}


            // Find out plugin properties according to interfaces it implements
            if (this.integratePlugin(gamePlugin) == false)
            {
                throw new PluginLoadFailedException("Error in plugin `" + pluginName +"': Plugin must be either a tile or an object.");
            }
        }

        private void OnLoadedRoundProperties(string roundName, int fieldsX, int fieldsY)
        {
            this.RoundName = roundName; // we want to notify
            this.fieldsX = fieldsX;
            this.fieldsY = fieldsY;
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
                gameObjects.Add(gamePlugin);
            }

            // Tiles
            IFixedTile fixedTile = gamePlugin as IFixedTile;

            if (fixedTile != null)
            {
                isElement = true;
                fixedTiles[fixedTile.PosX - 1, fixedTile.PosY - 1] = gamePlugin;
                gameObjects.Add(gamePlugin);
            }

            // Objects
            IMovableElement movableElement = gamePlugin as IMovableElement;

            if (movableElement != null)
            {
                isElement = true;
                movableElements.Add(gamePlugin);
                gameObjects.Add(gamePlugin);
            }

            return isElement;
        }
    }
}
