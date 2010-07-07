using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Sokoban.Lib.Exceptions;
using Sokoban.Lib;
using System.Threading;
using System.Net.NetworkInformation;

namespace Sokoban.Networking
{
    public class TcpConnection : DataBuffer
    {

        public const int MESSAGE_DESCRIPTOR_LENGTH = 4 + 4;
        protected byte[] outByteDescriptor = new byte[MESSAGE_DESCRIPTOR_LENGTH];
        protected Socket client;
        protected bool isInitialized = false;
        protected bool isClosed = false;
        private string role;

        public TcpConnection(string role)
        {
            this.role = role;
        }

        public bool IsInitialized
        {
            get { return isInitialized; }
        }


        private byte[] getByteDescriptorFromStream(MemoryStream stream, NetworkMessageType nmt)
        {
            byte[] outByteBuffer = new byte[stream.Length];
            stream.Read(outByteBuffer, 0, (int)stream.Length);

            BitConverter.GetBytes(outByteBuffer.Length).CopyTo(outByteDescriptor, 0);
            BitConverter.GetBytes((int)nmt).CopyTo(outByteDescriptor, 4);
            return outByteDescriptor;
        }

        //
        // Sending
        //

        private Queue<Triple<int, int, byte[]>> sendBuffer = new Queue<Triple<int, int, byte[]>>();
        protected Queue<Triple<int, int, byte[]>> SendBuffer { get { return sendBuffer; } }

        private bool isSending;
        private int sendingOrder;
        private EventWaitHandle allSentHandle;

        protected void sendingInit()
        {
            isSending = false;
            sendingOrder = 0;
            allSentHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            sendBuffer.Clear();
        }

        public EventWaitHandle AllSentHandle
        {
            get
            {
                bool succeded = allSentHandle.Reset();

                if (succeded == false)
                    throw new Exception("Cannot reset `AllSentHandle'.");

                if (sendBuffer.Count == 0)
                    allSentHandle.Set();

                return allSentHandle;
            }
        }

        /// <summary>
        /// Customer main function
        /// </summary>
        /// <param name="nmt"></param>
        public void SendAsync(NetworkMessageType nmt)
        {
            if (nmt == NetworkMessageType.ListOfEvents)
            {
                this.SendBufferedEvents();
            }
            else if (nmt == NetworkMessageType.Authentication)
            {
                this.SendAutentizationInfo();
            }
            else if (nmt == NetworkMessageType.DisconnectRequest)
            {
                this.SendDisconnectRequest();
            }
            else if (nmt == NetworkMessageType.DisconnectRequestConfirmation)
            {
                this.SendDisconnectRequestConfirmation();
            }
        }

        //
        // Handlers for sending network messages
        //

        private void sendRawData(NetworkMessageType nmt, object obj)
        {
            MemoryStream memStream = new MemoryStream();
            sendRawData(nmt, memStream, obj);
        }

        private void sendRawData(NetworkMessageType nmt, MemoryStream memStream, object obj)
        {
            binaryFormatter.Serialize(memStream, obj);

            byte[] byteDescriptor = getByteDescriptorFromStream(memStream, nmt);

            // Send the message descriptor
            this.SendAsync(byteDescriptor, byteDescriptor.Length);

            // Send the data
            byte[] data = memStream.ToArray();
            this.SendAsync(data, data.Length);
        }


        private void SendBufferedEvents()
        {
            this.sendRawData(NetworkMessageType.ListOfEvents, outBuffer);
        }

        private void SendDisconnectRequest()
        {
            DisconnectRequest disconnectRequest = new DisconnectRequest(DateTime.Now);
            this.sendRawData(NetworkMessageType.DisconnectRequest, disconnectRequest);
        }

        private void SendDisconnectRequestConfirmation()
        {
            DisconnectRequestConfirmation disconnectRequest = new DisconnectRequestConfirmation(DateTime.Now);
            this.sendRawData(NetworkMessageType.DisconnectRequestConfirmation, disconnectRequest);
        }


        private void SendAutentizationInfo()
        {
            Autentization autentization = new Autentization("Marty", "127.0.0.1");
            this.sendRawData(NetworkMessageType.Authentication, autentization);
        }

        //
        // END OF Handlers for sending network messages
        //


        //
        // Async sending of all messages in queue
        //

        private void SendAsync(byte[] data, int length)
        {
            sendingOrder++;
            sendBuffer.Enqueue(new Triple<int, int, byte[]>(sendingOrder, length, data));
            ProcessSendingQueue();
        }

        private void SendAsyncCallback(IAsyncResult ar)
        {
            //try
            //{
            // Retrieve the socket from the state object.
            //Socket handler = (Socket)ar.AsyncState;
            Socket handler = client;
            AsyncState asyncState = (AsyncState)ar.AsyncState;


            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);

            //Triple<int, int, byte[]> t = sendBuffer.Dequeue();

            if (bytesSent != asyncState.Length)
                throw new Exception(
                    "Exception: Sent bytes=" + bytesSent + "; Should be sent=" + asyncState.Length);

            DebuggerIX.WriteLine("[Net]", role,
                string.Format("Message #{0} was sent. Transfered {1} bytes.", asyncState.Order, asyncState.Length));
            //}
            //catch (Exception e)
            //{
            //   DebuggerIX.WriteLine("An error in SendCallback: " + e.ToString());
            //}

            ProcessSendingQueue();
        }

        private void ProcessSendingQueue()
        {
            if (sendBuffer.Count > 0)
            {
                isSending = true;
                Triple<int, int, byte[]> t = sendBuffer.Dequeue();
                AsyncState asyncState = new AsyncState(t.First, t.Second);
                client.BeginSend(t.Third, 0, t.Second, SocketFlags.None, new AsyncCallback(SendAsyncCallback), asyncState);
            }
            else
            {
                isSending = false;
                allSentHandle.Set();
            }
        }

        //
        // Asynchronous receiving
        //

        private enum receivingMode
        {
            none,
            receivingDescriptor,
            descriptorReceived,
            receivingMessage,
            messageReceived
        }

        private Queue<Pair<NetworkMessageType, byte[]>> rcvdBuffer = new Queue<Pair<NetworkMessageType, byte[]>>();

        public Queue<Pair<NetworkMessageType, byte[]>> RcvdBuffer
        {
            get { return rcvdBuffer; }
        }


        private receivingMode rcvdMode;
        private bool isReceiving;
        private int receivingOrder;
        private int rcvdBufferLength;
        private int rcvdBufferOffset;
        private byte[] rcvdByteBuffer = new byte[64536];
        private NetworkMessageType rcvdNetworkMessageType;

        protected void receivingInit()
        {
            rcvdBuffer.Clear();
            rcvdMode = receivingMode.none;
            isReceiving = false;
            receivingOrder = 0;
            rcvdBufferLength = 0;
            rcvdBufferOffset = 0;
        }


        public void ReceiveAsync()
        {
            ReceiveAsync(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceReceiving">Will run asynchronous process of receiving even though there may be a message in buffer</param>
        public void ReceiveAsync(bool forceReceiving)
        {
            if (forceReceiving == true || rcvdBuffer.Count == 0)
            {
                bool succeded = receivedMessageHandle.Reset();
                if (succeded == false)
                    throw new Exception("Cannot reset `ReceivedMessageHandle'.");

                receiveAsyncProcessing();
            }
            else
            {
                receivedMessageHandle.Set();
            }
        }

        public NetworkMessageType GetReceivedMessageType()
        {
            if (rcvdBuffer.Count == 0)
            {
                return NetworkMessageType.None;
            }
            else
            {
                Pair<NetworkMessageType, byte[]> message = rcvdBuffer.Peek();
                return message.First;
            }
        }

        public object GetReceivedMessageFromQueue()
        {
            if (rcvdBuffer.Count == 0)
            {
                return null;
            }
            else
            {
                Pair<NetworkMessageType, byte[]> message = rcvdBuffer.Dequeue();
                MemoryStream stream = new MemoryStream(message.Second, 0, message.Second.Length);

                if (message.First == NetworkMessageType.Authentication)
                {
                    return (Autentization)binaryFormatter.Deserialize(stream);
                }
                else if (message.First == NetworkMessageType.DisconnectRequest)
                {
                    return (DisconnectRequest)binaryFormatter.Deserialize(stream);
                }
                else if (message.First == NetworkMessageType.DisconnectRequestConfirmation)
                {
                    return (DisconnectRequestConfirmation)binaryFormatter.Deserialize(stream);
                }
                else
                {
                    throw new Exception("Unknown message type");
                }
            }
        }

        private void receiveAsyncProcessing()
        {

            isReceiving = true;

            if (rcvdMode == receivingMode.descriptorReceived)
            {
                int messageLength = BitConverter.ToInt32(rcvdByteBuffer, 0);
                rcvdNetworkMessageType = (NetworkMessageType)BitConverter.ToInt32(rcvdByteBuffer, 4);

                resetRcvdVariables();
                receiveMessage(messageLength);
            }
            else if (rcvdMode == receivingMode.messageReceived)
            {
                byte[] data = new byte[rcvdBufferLength];
                Buffer.BlockCopy(rcvdByteBuffer, 0, data, 0, rcvdBufferLength);
                rcvdBuffer.Enqueue(new Pair<NetworkMessageType, byte[]>(rcvdNetworkMessageType, data));

                receivedMessageHandle.Set();

                resetRcvdVariables();
                rcvdMode = receivingMode.none;
            }

            if (rcvdMode == receivingMode.none)
            {
                resetRcvdVariables();
                receiveDescriptor();
            }
        }

        private void resetRcvdVariables()
        {
            rcvdBufferLength = 0;
            rcvdBufferOffset = 0;
        }

        private void receiveDescriptor()
        {
            rcvdMode = receivingMode.receivingDescriptor;
            receivingOrder++;
            AsyncState asyncState = new AsyncState(receivingOrder, MESSAGE_DESCRIPTOR_LENGTH);
            try
            {
                client.BeginReceive(rcvdByteBuffer, 0, MESSAGE_DESCRIPTOR_LENGTH, 0,
                    new AsyncCallback(ReadCallback), asyncState);
            }
            catch (SocketException e)
            {
                DebuggerIX.WriteLine("[Net]", role, "SocketException: " + e.Message);
            }
        }

        private void receiveMessage(int length)
        {
            rcvdMode = receivingMode.receivingMessage;
            receivingOrder++;

            // Reallocating if needed
            if (length > rcvdByteBuffer.Length) rcvdByteBuffer = new byte[length];

            AsyncState asyncState = new AsyncState(receivingOrder, length);
            client.BeginReceive(rcvdByteBuffer, 0, length, 0, new AsyncCallback(ReadCallback), asyncState);
        }

        public void ReadCallback(IAsyncResult ar)
        {

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            AsyncState asyncState = (AsyncState)ar.AsyncState;


            if (isClosed == true) return;
            // Read data from the client socket. 
            int received = client.EndReceive(ar);
            rcvdBufferLength += received;
            rcvdBufferOffset += rcvdBufferLength;

            if (received == asyncState.Length)
            {
                // All the data has been read from the 
                // client. Display it on the console.
                //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);                

                if (rcvdMode == receivingMode.receivingDescriptor)
                {
                    DebuggerIX.WriteLine("[Net]", role,
                        "Message #" + asyncState.Order + " descriptor received. Total bytes: " + rcvdBufferLength +
                        "; Next message (length, type): (" + BitConverter.ToInt32(rcvdByteBuffer, 0) +
                        ", " + (NetworkMessageType)BitConverter.ToInt32(rcvdByteBuffer, 4) + ")");
                    rcvdMode = receivingMode.descriptorReceived;
                }
                else if (rcvdMode == receivingMode.receivingMessage)
                {
                    DebuggerIX.WriteLine("[Net]", role, 
                        "Message #" + asyncState.Order + " received. Total bytes: " + rcvdBufferLength);
                    rcvdMode = receivingMode.messageReceived;
                }

                receiveAsyncProcessing();
            }
            else
            {
                asyncState.Length = asyncState.Length - received;

                // Not all data received. Get more.
                try
                {
                    client.BeginReceive(rcvdByteBuffer, rcvdBufferOffset, (asyncState.Length - received), SocketFlags.None,
                        new AsyncCallback(ReadCallback), asyncState);
                }
                catch (SocketException e)
                {
                    DebuggerIX.WriteLine("[Net]", role, "ReadCallback: Exception. Message: " + e.Message);
                }

                //} 
                //catch (SocketException e)
                //{
                //if (isClosed == false) throw new Exception("SocketException: " + e.Message);
                //}
            }

        }

        private EventWaitHandle receivedMessageHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        public EventWaitHandle ReceivedMessageHandle
        {
            get
            {
                return receivedMessageHandle;
            }
        }

        //
        // Async diconnection
        //

        private EventWaitHandle disconnectDoneHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public virtual void CloseConnection()
        {
            this.CloseConnection(this.client);
        }

        public virtual void CloseConnection(Socket client)
        {
            if (client != null)
            {
                isClosed = true;
                if (client.Connected)
                {
                    disconnectDoneHandle.Reset();
                    client.Shutdown(SocketShutdown.Both);
                    client.BeginDisconnect(false, new AsyncCallback(DisconnectCallback), client);

                    // Wait for the disconnect to complete.
                    disconnectDoneHandle.WaitOne();
                }
            }
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            // Complete the disconnect request.
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);
            isInitialized = false;

            // Signal that the disconnect is complete.
            disconnectDoneHandle.Set();
        }


        //
        // Other useful functions
        //

        public bool IsPortAvailable(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }
    }

    class AsyncState
    {
        public int Order { get; set; }
        public int Length { get; set; }
        public AsyncState(int order, int length)
        {
            Order = order;
            Length = length;
        }
    }
}
