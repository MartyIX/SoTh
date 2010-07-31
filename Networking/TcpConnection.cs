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
        protected Socket clientSocket;
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

            lock (sendAsyncCallbackLock)
            {
                sendBuffer.Clear();
            }
        }

        public EventWaitHandle AllSentHandle
        {
            get
            {
                bool succeded = allSentHandle.Reset();

                if (succeded == false)
                    throw new Exception("Cannot reset `AllSentHandle'.");

                lock (sendAsyncCallbackLock)
                {
                    if (sendBuffer.Count == 0)
                        allSentHandle.Set();
                }

                return allSentHandle;
            }
        }

        /// <summary>
        /// Customer main function
        /// </summary>
        /// <param name="nmt"></param>
        public void SendAsync(NetworkMessageType nmt)
        {
            SendAsync(nmt, null);
        }

        public void SendAsync(NetworkMessageType nmt, object data)
        {
            if (nmt == NetworkMessageType.ListOfEvents)
            {
                if (outBuffer.Count > 0)
                {
                    this.sendRawData(NetworkMessageType.ListOfEvents, new ListOfEventsMessage((long)data, outBuffer));
                    outBuffer.Clear();
                }
            }
            else if (nmt == NetworkMessageType.GameChange)
            {
                if (data == null)
                {
                    throw new InvalidStateException("SendAsync: data is null; expected GameWonMessage instance.");
                }

                this.sendRawData(NetworkMessageType.GameChange, (GameChangeMessage)data);
            }
            else if (nmt == NetworkMessageType.Authentication)
            {
                if (data == null)
                {
                    throw new InvalidStateException("SendAsync: data is null; expected Autentization instance.");
                }

                this.sendRawData(NetworkMessageType.Authentication, (Authentication)data);
            }
            else if (nmt == NetworkMessageType.SimulationTime)
            {
                if (data == null)
                {
                    throw new InvalidStateException("SendAsync: data is null; expected SimulationTimeMessage instance.");
                }
                
                this.sendRawData(NetworkMessageType.SimulationTime, (ListOfEventsMessage)data);
            }
            else if (nmt == NetworkMessageType.DisconnectRequest)
            {
                DisconnectRequest disconnectRequest = new DisconnectRequest(DateTime.Now);
                this.sendRawData(NetworkMessageType.DisconnectRequest, disconnectRequest);
            }
            else if (nmt == NetworkMessageType.DisconnectRequestConfirmation)
            {
                DisconnectRequestConfirmation disconnectRequest = new DisconnectRequestConfirmation(DateTime.Now);
                this.sendRawData(NetworkMessageType.DisconnectRequestConfirmation, disconnectRequest);
            }
            else if (nmt == NetworkMessageType.StartGame)
            {
                StartGame startGame = new StartGame(DateTime.Now);
                this.sendRawData(NetworkMessageType.StartGame, startGame);
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



        //
        // END OF Handlers for sending network messages
        //


        //
        // Async sending of all messages in queue
        //

        private void SendAsync(byte[] data, int length)
        {
            lock (sendAsyncCallbackLock)
            {
                sendingOrder++;            
                sendBuffer.Enqueue(new Triple<int, int, byte[]>(sendingOrder, length, data));
                ProcessSendingQueue();
            }
        }


        private object sendAsyncCallbackLock = new object();

        /// <summary>
        /// May be called from several threads at the same time. THREAD SAFETY IS REQUIRED!!
        /// </summary>
        /// <param name="ar"></param>
        private void SendAsyncCallback(IAsyncResult ar)
        {
            lock (sendAsyncCallbackLock)
            {
                //try
                //{
                // Retrieve the socket from the state object.
                //Socket handler = (Socket)ar.AsyncState;
                Socket handler = clientSocket;
                AsyncState asyncState = (AsyncState)ar.AsyncState;
                int bytesSent = 0;

                try
                {
                    // Complete sending the data to the remote device.
                    bytesSent = handler.EndSend(ar);

                }
                catch (NullReferenceException e)
                {
                    if (isClosed == false)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, e.Message);
                    }

                    return;
                }
                catch (SocketException e)
                {
                    if (isClosed == false)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, e.Message);
                    }

                    return;
                }

                //Triple<int, int, byte[]> t = sendBuffer.Dequeue();

                if (bytesSent != asyncState.Length)
                    throw new Exception(
                        "Exception: Sent bytes=" + bytesSent + "; Should be sent=" + asyncState.Length);

                DebuggerIX.WriteLine(DebuggerTag.Net, role,
                    string.Format("Message #{0} was sent. Transfered {1} bytes.", asyncState.Order, asyncState.Length));
                //}
                //catch (Exception e)
                //{
                //   DebuggerIX.WriteLine("An error in SendCallback: " + e.ToString());
                //}

                ProcessSendingQueue();
            }
        }

        /// <summary>
        /// called in locked area: sendAsyncCallbackLock
        /// </summary>
        private void ProcessSendingQueue()
        {
            Triple<int, int, byte[]> t = null;

            if (sendBuffer.Count > 0)
            {
                t = sendBuffer.Dequeue();
            }
        
            if (t != null)
            {
                isSending = true;
                AsyncState asyncState = new AsyncState(t.First, t.Second);
                try
                {
                    clientSocket.BeginSend(t.Third, 0, t.Second, SocketFlags.None, new AsyncCallback(SendAsyncCallback), asyncState);
                }
                catch (SocketException e)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "ProcessSendingQueue, SocketException: " + e.Message);
                    throw;
                }
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

        private object lockRcvdBuffer = new object();
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
            lock (readCallbackLock)
            {
                rcvdBuffer.Clear();
            }

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
            lock (readCallbackLock)
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

                if (Enum.IsDefined(typeof(NetworkMessageType), message.First) && 
                    message.First != NetworkMessageType.None)
                {
                    return binaryFormatter.Deserialize(stream);
                }
                else
                {
                    throw new Exception("Unknown message type: " + message.First.ToString());
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
                clientSocket.BeginReceive(rcvdByteBuffer, 0, MESSAGE_DESCRIPTOR_LENGTH, 0,
                    new AsyncCallback(ReadCallback), asyncState);
            }
            catch (SocketException e)
            {
                DebuggerIX.WriteLine(DebuggerTag.Net, role, "SocketException: " + e.Message);
                throw;
            }
        }

        private void receiveMessage(int length)
        {
            rcvdMode = receivingMode.receivingMessage;
            receivingOrder++;

            // Reallocating if needed

            if (length < 1000000)
            {

                if (length > rcvdByteBuffer.Length) rcvdByteBuffer = new byte[length];

                AsyncState asyncState = new AsyncState(receivingOrder, length);
                try
                {
                    clientSocket.BeginReceive(rcvdByteBuffer, 0, length, 0, new AsyncCallback(ReadCallback), asyncState);
                }
                catch (SocketException se)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "SocketException: " + se.Message);
                    throw;
                }
            }
            else
            {
                throw new SocketException();
            }
        }

        private object readCallbackLock = new object();
        
        public void ReadCallback(IAsyncResult ar)
        {
            lock (readCallbackLock)
            {

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                AsyncState asyncState = (AsyncState)ar.AsyncState;

                // Read data from the client socket. 
                int received = 0;

                try
                {
                    received = clientSocket.EndReceive(ar);
                }
                catch (ObjectDisposedException)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "ReadCallback: Socket has been closed\n");
                    return;
                }
                catch (ArgumentException e)
                {
                    DebuggerIX.WriteLine(DebuggerTag.Net, role, "ReadCallback: Called from wrong AsyncResult; Exception: " + e.Message);
                    return;
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "Client  Disconnected");
                        return;
                    }
                    else
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, se.Message);
                        return;
                    }
                }

                rcvdBufferLength += received;
                rcvdBufferOffset += rcvdBufferLength;

                if (received == asyncState.Length)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);                

                    if (rcvdMode == receivingMode.receivingDescriptor)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role,
                            "Message #" + asyncState.Order + " descriptor received. Total bytes: " + rcvdBufferLength +
                            "; Next message (length, type): (" + BitConverter.ToInt32(rcvdByteBuffer, 0) +
                            ", " + (NetworkMessageType)BitConverter.ToInt32(rcvdByteBuffer, 4) + ")");
                        rcvdMode = receivingMode.descriptorReceived;
                    }
                    else if (rcvdMode == receivingMode.receivingMessage)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role,
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
                        clientSocket.BeginReceive(rcvdByteBuffer, rcvdBufferOffset, asyncState.Length, SocketFlags.None,
                            new AsyncCallback(ReadCallback), asyncState);
                    }
                    catch (SocketException e)
                    {
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "ReadCallback: Exception. Message: " + e.Message);
                    }

                    //} 
                    //catch (SocketException e)
                    //{
                    //if (isClosed == false) throw new Exception("SocketException: " + e.Message);
                    //}
                }
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
            this.CloseConnection(this.clientSocket);
        }

        public virtual void CloseConnection(Socket clientSocket)
        {
            lock (sendAsyncCallbackLock)
            {
                lock (readCallbackLock)
                {
                    if (clientSocket != null)
                    {
                        isClosed = true;
                        if (clientSocket.Connected)
                        {
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Connection Shutdown");
                            clientSocket.Shutdown(SocketShutdown.Both);
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Beginning disconnect");
                            //clientSocket.Disconnect(true);  // 10 second timeout
                            clientSocket.Close(); // http://stackoverflow.com/questions/583637/c-net-socket-shutdown

                            if (clientSocket.Connected)
                            {
                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "We're still connnected");
                                isInitialized = true;
                            }
                            else
                            {
                                DebuggerIX.WriteLine(DebuggerTag.Net, role, "We're disconnected");
                                isInitialized = false;
                            }

                            clientSocket = null;
                        }
                        else
                        {
                            isInitialized = false;
                            DebuggerIX.WriteLine(DebuggerTag.Net, role, "Already closed.");
                        }
                    }
                    else
                    {
                        isInitialized = false;
                        DebuggerIX.WriteLine(DebuggerTag.Net, role, "Already closed.");
                    }
                }
            }
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


        //
        // Waiting methods
        //        

        protected void waitHandle(IAsyncResult asyncResult)
        {
            waitHandle(asyncResult, -1); // -1 = indefinitely
        }

        protected void waitHandle(IAsyncResult asyncResult, int timeoutMilliseconds)
        {
            if (!asyncResult.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                throw new TimeoutException();
            }
        }

        protected void waitHandle(ManualResetEvent manualResetEvent, int timeoutMilliseconds)
        {
            if (!manualResetEvent.WaitOne(timeoutMilliseconds))
            {
                throw new TimeoutException();
            }
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
