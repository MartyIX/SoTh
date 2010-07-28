using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public DateTime SentTime { get; set; }
        public Int64 SimulationTime { get; set; }

        public SimulationTimeMessage(Int64 simulationTime, DateTime sentTime)
        {
            SentTime = sentTime;
            SimulationTime = simulationTime;
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
