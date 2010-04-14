using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace Sokoban
{

    [TestFixture]
    public class NetworkMessageProtocolTests
    {
        // TEST 1 
        // ===============================================================================================
        
        int NMP_1_receivedEvents = 0;

        public void NMP_1_OnEventWasRead(int eventID, int objectID, Int64 when, EventType what, int posX, int posY)
        {
            NMP_1_receivedEvents++;
        }
        
        [Test]
        public void NMP_1()  // test if number of sent messages is equal to number of received messages
        {
            NetworkMessageProtocol nmp = new NetworkMessageProtocol();
            Queue<Event> queue = new Queue<Event>();
            byte[] messageToSent = new byte[1200];
            int messageLen;
            int iterationNo;
            Int64 actualSimulationTime;  
            GameObject gameObject = new GameObject();

            nmp.EventWasRead += new d_SimulationEventHandler(this.NMP_1_OnEventWasRead);
            actualSimulationTime = 22;
            iterationNo = 10;
            NMP_1_receivedEvents = 0;

            // generate a few events
            for (int i = 0; i < iterationNo; i++)
            {
                queue.Enqueue(new Event(i, i, gameObject, EventType.none));
            }

            // Prepare events in queue for sending; result is in "messageToRead"
            nmp.EncodeEvents(ref queue, ref messageToSent, out messageLen, actualSimulationTime);

            // Reading 
            nmp.DecodeData(messageToSent, messageLen);

            Debug.Write("\nRESULT: " +NMP_1_receivedEvents.ToString() + " is equal to " + iterationNo.ToString() + " ? \n\n");
            Assert.That(NMP_1_receivedEvents == iterationNo);
        }

        // TEST 2 
        // ===============================================================================================

        Int64 NMP_2_SentTime = 1234;

        public void NMP_2_OnNetworkTimeChanged(Int64 newTime)
        {
            Debug.Write("\nRESULT: " + newTime.ToString() + " is equal to " + NMP_2_SentTime.ToString() + " ? \n\n");
            Assert.That(newTime == NMP_2_SentTime);
        }


        [Test]
        public void NMP_2()  // Was time transfered OK?
        {
            NetworkMessageProtocol nmp = new NetworkMessageProtocol();
            Queue<Event> queue = new Queue<Event>();
            byte[] messageToSent = new byte[1200];
            int messageLen;
            GameObject gameObject = new GameObject();

            nmp.NetworkTimeChanged += new d_NetworkTimeChanged(NMP_2_OnNetworkTimeChanged);

            queue.Enqueue(new Event(1, 1, gameObject, EventType.none));

            // Prepare events in queue for sending; result is in "messageToRead"
            nmp.EncodeEvents(ref queue, ref messageToSent, out messageLen, NMP_2_SentTime);

            // Reading 
            nmp.DecodeData(messageToSent, messageLen);
        }


        // TEST 3
        // ===============================================================================================

        int NMP_3_eventID = 7;
        Int64 NMP_3_when = 150;
        GameObject NMP_3_gameObject = new GameObject();
        EventType NMP_3_eventType = EventType.guardColumn;
        int NMP_3_posX = 4;
        int NMP_3_posY = 5;
        int NMP_3_receivedEvents = 0;

        public void NMP_3_OnEventWasRead(int eventID, int objectID, Int64 when, EventType what, int posX, int posY)
        {
            NMP_3_receivedEvents++;


            Debug.Write("\nRESULT #1: " + NMP_3_receivedEvents.ToString() + " should be 1. \n\n");
            Assert.That(NMP_3_eventID == eventID);
            Assert.That(NMP_3_when == when);
            Assert.That(NMP_3_gameObject.objectID == objectID);
            Assert.That(NMP_3_eventType == what);
            Assert.That(NMP_3_posX == posX);
            Assert.That(NMP_3_posY == posY);
        }

        [Test]
        public void NMP_3()  // Was all data of event sent and received OK?
        {
            NetworkMessageProtocol nmp = new NetworkMessageProtocol();
            Queue<Event> queue = new Queue<Event>();
            byte[] messageToSent = new byte[1200];
            int messageLen;
            GameObject gameObject = new GameObject();

            nmp.EventWasRead += new d_SimulationEventHandler(NMP_3_OnEventWasRead);

            queue.Enqueue(new Event(NMP_3_eventID, NMP_3_when, NMP_3_gameObject, NMP_3_eventType, NMP_3_posX, NMP_3_posY));

            // Prepare events in queue for sending; result is in "messageToRead"
            nmp.EncodeEvents(ref queue, ref messageToSent, out messageLen, NMP_2_SentTime);

            // Reading 
            nmp.DecodeData(messageToSent, messageLen);

            Debug.Write("\nRESULT #2: " + NMP_3_receivedEvents.ToString() + " should be 1. \n\n");
            Assert.That(NMP_3_receivedEvents == 1);
        }

    }
}