using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Sokoban.Lib.Exceptions;

namespace Sokoban.Networking
{
    public class TcpConnection : DataBuffer
    {
        public const int MESSAGE_DESCRIPTOR_LENGTH = 8 + 4;
        public byte[] inByteBuffer = new byte[65436];
        private long inMessageLength = 0;
        protected byte[] outByteDescriptor = new byte[MESSAGE_DESCRIPTOR_LENGTH];
        protected Socket client;
        protected bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }
        

        /// <summary>
        /// returns outByteBuffer and sets outByteDescriptor
        /// </summary>
        /// <returns></returns>
        private byte[] prepareBufferedEvents()
        {
            // Save data to outMemoryStream
            MemoryStream outMemoryStream = new MemoryStream();
            this.SerializeData_In_OutBuffer(outMemoryStream);
            byte[] outByteBuffer = outMemoryStream.ToArray();

            BitConverter.GetBytes(outMemoryStream.Length).CopyTo(outByteDescriptor, 0);         
            BitConverter.GetBytes((int)NetworkMessageType.ListOfEvents).CopyTo(outByteDescriptor, 8);

            return outByteBuffer;
        }

        /// <summary>
        /// readMessageDescriptor from received data and set @inMessageLength variable
        /// </summary>
        /// <returns></returns>
        private NetworkMessageType readMessageDescriptor()
        {
            int totalBytesRcvd = 0;      //  Total  bytes  received  so  far
            int bytesRcvd = 0; //  Bytes  received  in  last  read

            // Message descriptor
            while (totalBytesRcvd < MESSAGE_DESCRIPTOR_LENGTH)
            {
                if ((bytesRcvd = client.Receive(inByteBuffer, totalBytesRcvd,
                inByteBuffer.Length - totalBytesRcvd, SocketFlags.None)) == 0)
                {
                    throw new NetworkTransferException("Connection  closed  prematurely.");
                }
                totalBytesRcvd += bytesRcvd;
            }

            inMessageLength = BitConverter.ToInt64(inByteBuffer, 0);
            NetworkMessageType nmt = (NetworkMessageType)BitConverter.ToInt32(inByteBuffer, 8);

            return nmt;
        }

        private void SendBufferedEvents()
        {
            // It sets outByteDescriptor
            byte[] outByteBuffer = this.prepareBufferedEvents();

            // Send the message descriptor
            client.Send(outByteDescriptor, 0, outByteDescriptor.Length, SocketFlags.None);

            // Send the message itself
            client.Send(outByteBuffer, 0, outByteBuffer.Length, SocketFlags.None);
        }

        public void Send(NetworkMessageType nmt)
        {
            if (nmt == NetworkMessageType.ListOfEvents)
            {
                this.SendBufferedEvents();
            }
        }

        public NetworkMessageType Receive()
        {
            // Receive the message descriptor
            NetworkMessageType nmt = this.readMessageDescriptor();

            if (nmt == NetworkMessageType.ListOfEvents)
            {
                int totalBytesRcvd = 0; //  Total  bytes  received  so  far
                int bytesRcvd = 0;      //  Bytes  received  in  last  read            

                //  Receive the message itself
                while (totalBytesRcvd < this.inMessageLength)
                {
                    if ((bytesRcvd = client.Receive(inByteBuffer, totalBytesRcvd, inByteBuffer.Length - totalBytesRcvd, SocketFlags.None)) == 0)
                    {
                        throw new NetworkTransferException("Connection  closed  prematurely.");
                    }
                    totalBytesRcvd += bytesRcvd;
                }

                this.DeserializeData_To_InBuffer(inByteBuffer, totalBytesRcvd);         
            }

            return nmt;
        }

        public void CloseConnection()
        {
            if (client != null)
            {
                client.Close();
            }
        }

    }
}
