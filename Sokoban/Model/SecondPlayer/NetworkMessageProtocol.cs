using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public class NetworkMessageProtocol
    {
        public event d_NetworkTimeChanged NetworkTimeChanged;
        public event d_SimulationEventHandler EventWasRead;
        public event d_PlayerFinishedRound PlayerFinishedRound;

        public void DecodeData(byte[] data, int received)
        {
            int processed = 0;
            int packageType = CommonFunc.Block2Int32(data, ref processed);                   // Package type

            if (packageType == 111) // receiving events of opponents game
            {
                int numberOfSentEvents = CommonFunc.Block2Int32(data, ref processed);        // number of events
                Int64 time = CommonFunc.Block2Int64(data, ref processed);

                if (NetworkTimeChanged != null) NetworkTimeChanged(time);               // EVENT!!

                for (int i = 0; i < numberOfSentEvents; i++)
                {
                    int eventID = CommonFunc.Block2Int32(data, ref processed);
                    int objectID = CommonFunc.Block2Int32(data, ref processed);              // objectID
                    Int64 when = CommonFunc.Block2Int64(data, ref processed);                // when
                    EventType what = (EventType)CommonFunc.Block2Int32(data, ref processed); // what

                    int posX = CommonFunc.Block2Int32(data, ref processed);                  // PosX
                    int posY = CommonFunc.Block2Int32(data, ref processed);                  // PosY

                    if (EventWasRead != null)
                        EventWasRead(eventID, objectID, when, what, posX, posY);             // Invoking all the event handlers
                }
            }
            else if (packageType == 222) // victory message
            {
                Int64 ticks = CommonFunc.Block2Int64(data, ref processed);

                if (PlayerFinishedRound != null) PlayerFinishedRound(ticks);                 // Invoking all the event handlers                
            }
            else
            {
                GameDeskView.Debug("Received unknown packageType: #" + packageType.ToString(), "NetworkReceiving");
            }
        }

        public int EncodeEvents(ref Queue<Event> eventBuffer, ref byte[] messageToSend, out int messageLen, Int64 actualSimulationTime)
        {
            int offset = 0; // at the beginning of the "packet" (message) will be written number of events that are being sent
            int processed = 0;

            CommonFunc.BlockCopyEx(messageToSend, (int)111, ref offset); // package of messages #1 (sending of events); this affects how the package is read
            offset += 4; // for number of events that are going to be sent (look below)
            CommonFunc.BlockCopyEx(messageToSend, actualSimulationTime, ref offset); // synchronizing time

            lock (eventBuffer)
            {
                // events
                while (eventBuffer.Count > 0)
                {
                    if (offset > 1000) break;

                    Event ev = eventBuffer.Dequeue();
                    processed++;

                    CommonFunc.BlockCopyEx(messageToSend, ev.EventID, ref offset);
                    CommonFunc.BlockCopyEx(messageToSend, ev.who.objectID, ref offset);
                    CommonFunc.BlockCopyEx(messageToSend, ev.when, ref offset);
                    CommonFunc.BlockCopyEx(messageToSend, (int)(ev.what), ref offset);
                    CommonFunc.BlockCopyEx(messageToSend, ev.posX, ref offset);
                    CommonFunc.BlockCopyEx(messageToSend, ev.posY, ref offset);
                }

                // number of events sent:

                int tmpOffset = 4;
                CommonFunc.BlockCopyEx(messageToSend, processed, ref tmpOffset);
            }

            messageLen = offset;

            return processed;
        }

        public int EncodeVictoryMessage(ref byte[] messageToSend, Int64 ticks)
        {
            int messageLen = 0;
            EncodeVictoryMessage(ref messageToSend, out messageLen, ticks);
            return messageLen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageToSend"></param>
        /// <param name="messageLen"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public void EncodeVictoryMessage(ref byte[] messageToSend, out int messageLen, Int64 ticks)
        {
            int offset = 0; // at the beginning of the "packet" (message) will be written number of events that are being sent

            CommonFunc.BlockCopyEx(messageToSend, (int)222, ref offset); // package of messages #1 (sending of events); this affects how the package is read
            CommonFunc.BlockCopyEx(messageToSend, ticks, ref offset); // synchronizing time

            messageLen = offset;
        }    
    }
}
