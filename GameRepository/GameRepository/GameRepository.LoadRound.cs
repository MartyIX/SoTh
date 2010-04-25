using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.GameDesk;
using System.Collections;
using Sokoban.Lib;

namespace Sokoban.Model
{
    using Sokoban.Model.PluginInterface;

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
            functionalTiles = new List<IGamePlugin>();
            movableObstructionObjects = new List<IGamePlugin>();
            controllableByUserObjects = null;
            solidObstructionObjects = new bool[fieldsX, fieldsY];
            tableFunctionalTiles = new IGamePlugin[fieldsX, fieldsY];


            //Call the find plugins routine, to search in our Plugins Folder
            this.FindPlugins(@"D:\Bakalarka\Sokoban\Main\Plugins");

            // Reading from the XML 
            XmlRoundReader xmlRounds = new XmlRoundReader(xml);
            xmlRounds.RoundPropertiesRead += new RoundPropertiesDelegate(OnLoadedRoundProperties);
            xmlRounds.GameObjectRead += new GameObjectPropertiesDelegate(OnLoadedGameObject);
            xmlRounds.LoadRoundSettings();

            // Setting walls
            for (int j = 0; j < fieldsY; j++)
            {
                for (int i = 0; i < fieldsX; i++)
                {
                    solidObstructionObjects[i, j] = false;
                    tableFunctionalTiles[i, j] = null;
                }
            }

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

        private void OnLoadedGameObject(int objectID, string description, int posX, int posY, MovementDirection direction, EventType InitialEvent, int speed)
        {
            // Default speed if no speed or negative speed is in XML
            if (speed <= 0)
            {
                speed = 3;
            }

            if (description == "S")
            {
                Types.AvailablePlugin selectedPlugin = AvailablePlugins.Find("Sokoban");

                gameObjects.Add(selectedPlugin.Instance);

                //SokobanObj obj = new SokobanObj(objectID, description, posX, posY, direction, speed);
                /*
                logList.SaveInitPositions(obj, obj.PozX, obj.PozY, obj.direction);
                logList.AddEvent(obj, obj.posX, obj.posY, "0:00", obj.direction, true);
                */

                //gameObjects.Add(obj);
                //MakeImmediatePlan("LoadRndSokInitEv", obj, InitialEvent);
                //controllableByUserObjects = obj;
            }
            else
            {
                /*
                GameObject obj = new GameObject(objectID, description, posX, posY, direction, speed);
                //logList.SaveInitPositions(obj, obj.PozX, obj.PozY, obj.direction);

                gameObjects.Add(obj);
                MakeImmediatePlan("LoadRndObjInitEv", obj, InitialEvent);

                if (description == "B")
                {
                    movableObstructionObjects.Add(obj);
                }
                else if (description == "W")
                {
                    solidObstructionObjects[posX - 1, posY - 1] = true;
                }
                else if (description == "A")
                {
                    functionalTiles.Add(obj);
                    tableFunctionalTiles[posX - 1, posY - 1] = obj;
                } */                   
            }
        }

        private void OnLoadedRoundProperties(string roundName, int fieldsX, int fieldsY)
        {
            this.roundName = roundName;
            this.fieldsX = fieldsX;
            this.fieldsY = fieldsY;
        }
    }
}
