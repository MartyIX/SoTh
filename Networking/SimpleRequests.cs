using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;

namespace Sokoban.Networking
{
    [Serializable]
    public class DisconnectRequest
    {
        public DateTime DateTime { get; set; }

        public DisconnectRequest(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }

    [Serializable]
    public class SimulationTimeMessage
    {
        public TimeSpan ElapsedFromStartOfGame { get; set; }
        public Int64 SimulationTime { get; set; }

        public SimulationTimeMessage(Int64 simulationTime, TimeSpan elapsedFromStartOfGame)
        {
            ElapsedFromStartOfGame = elapsedFromStartOfGame;
            SimulationTime = simulationTime;
        }
    }

    [Serializable]
    public class ListOfEventsMessage
    {
        public Queue<NetworkEvent> Events { get; set; }
        public Int64 SimulationTime { get; set; }

        public ListOfEventsMessage(Int64 simulationTime, Queue<NetworkEvent> events)
        {
            Events = events;
            SimulationTime = simulationTime;
        }
    }


    [Serializable]
    public class GameChangeMessage
    {
        public TimeSpan ElapsedFromStartOfGame { get; set; }
        public GameChange GameChange { get; set; }

        public GameChangeMessage(GameChange gameChange, TimeSpan elapsedFromStartOfGame)
        {
            ElapsedFromStartOfGame = elapsedFromStartOfGame;
            GameChange = gameChange;
        }
    }


    [Serializable]
    public class DisconnectRequestConfirmation
    {
        public DateTime DateTime { get; set; }

        public DisconnectRequestConfirmation(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }

    [Serializable]
    public class StartGame
    {
        public DateTime DateTime { get; set; }

        public StartGame(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }


}
