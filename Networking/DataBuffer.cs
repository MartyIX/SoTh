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
	}
}
