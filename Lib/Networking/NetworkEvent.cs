using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Networking
{
    [Serializable]
    public struct NetworkEvent
    {
        /// <summary>
        /// Events are numbered when they are sent
        /// </summary>
        public int OrderID;

        /// <summary>
        /// When to process the event
        /// </summary>
        public Int64 SimulationTime;

        /// <summary>
        /// When to process the event
        /// </summary>
        public DateTime Time;

        /// <summary>
        /// Index to GameRepository.gameObjects list
        /// </summary>
        public int GameObjectID;

        /// <summary>
        /// Type of event
        /// </summary>
        public EventType EventType;

        public int PosX;
        public int PosY;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderID">Identifier of the event - for debugging purposes</param>
        /// <param name="simulationTime">When to process the event</param>
        /// <param name="who">Which object should process the event</param>
        /// <param name="what">Type of event</param>
        public NetworkEvent(int orderID, Int64 simulationTime, int gameObjectID, EventType what, int posX, int posY)
        {
            this.Time = DateTime.Now;
            this.OrderID = orderID;
            this.SimulationTime = simulationTime;
            this.GameObjectID = gameObjectID;
            this.EventType = what;
            this.PosX = posX;
            this.PosY = posY;
        }

        public override string ToString()
        {
            return string.Format("#{0} | Time={1}; Simulation time={2}; GameObjectID={3}; EventType={4}; PosX={5}; PosY={6}",
                this.OrderID, this.Time, this.SimulationTime, this.GameObjectID, this.EventType, this.PosX, this.PosY);
        }
         
    }
}
