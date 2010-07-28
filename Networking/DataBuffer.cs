using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Sokoban.Lib.Exceptions;
using System.Runtime.Serialization;

namespace Sokoban.Networking
{
    public class DataBuffer
    {
        protected Queue<NetworkEvent> inBuffer; // incoming buffer
        protected Queue<NetworkEvent> outBuffer; // outcomming buffer
        private int orderID;
        protected BinaryFormatter binaryFormatter = new BinaryFormatter();

        public DataBuffer()
        {
            inBuffer = new Queue<NetworkEvent>();
            outBuffer = new Queue<NetworkEvent>();
            orderID = 0;
        }

        public void AddEventToBuffer(Int64 time, int gameObjectID, EventType what, int posX, int posY)
        {
            orderID++;
            NetworkEvent ne = new NetworkEvent(orderID, time, gameObjectID, what, posX, posY);
            outBuffer.Enqueue(ne);
        }

        public int InBufferCount
        {
            get
            {
                return inBuffer.Count;
            }
        }

        public NetworkEvent GetIncommingEvent()
        {
            if (inBuffer.Count > 0)
            {
                return inBuffer.Dequeue();
            }
            else
            {
                throw new Exception("No more messages in incoming buffer");
            }
        }


        public void DeserializeData_To_InBuffer(byte[] data, int length)
        {
            MemoryStream memoryStream = new MemoryStream(data, 0, length);
            DeserializeData_To_InBuffer(memoryStream);
        }

        public void DeserializeData_To_InBuffer(Stream stream)
        {
            BinaryFormatter bin = new BinaryFormatter();
            Queue<NetworkEvent> q = null;
            try
            {
                q = (Queue<NetworkEvent>)binaryFormatter.Deserialize(stream);
            }
            catch (SerializationException e)
            {
                throw new NetworkTransferException("Data transfered over network are corrupted. " +
                    "The error is unrecoverable. Error: " + e.Message);
            }

            while (q != null && q.Count > 0)
	        {
                NetworkEvent ne = q.Dequeue();
                inBuffer.Enqueue(ne);
                DebuggerIX.WriteLine(DebuggerTag.Net, "[DeserializeData]", ne.ToString());
	        }                               
        }
    }
}
