namespace ClientServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketClient
    {
        private readonly TcpClient _clientSocket;
        private byte[] _incompleteBuffer;
        private const string WriteBufferEof = "~_-+=EOF=+-_~";

        public SocketClient(TcpClient client)
        {
            _clientSocket = client;
        }

        public Guid Id { get; set; }

        public bool Connected
        {
            get { return _clientSocket.Connected; }
        }

        public void StartClient()
        {
            ListenForMessages();
        }
        private void ListenForMessages()
        {
            Task.Run(() =>
            {
                while (Connected)
                {
                    if (_clientSocket.Connected && _clientSocket.Available > 0)
                    {
                        var messages = ReadMessages();
                        foreach (var message in messages)
                        {
                            var formatter = new BinaryFormatter();
                            var networkMessage = (NetworkMessage)formatter.Deserialize(message);

                            if (networkMessage.MessageType != NetworkMessageType.KeepAliveMessage)
                            {
                                Task.Run(() => OnClientMessage(this, networkMessage));
                            }
                        }
                    }
                    Thread.Sleep(250); // i dont want to read all the time
                }
                Close();
            });
        }

        private List<MemoryStream> ReadMessages()
        {
            var buffer = GetBuffer();

            return GetMessageStreams(buffer);
        }

        private byte[] GetBuffer()
        {
            var buffer = new byte[1024];
            var input = new byte[0];

            using (var networkStream = new NetworkStream(_clientSocket.Client))
            {
                while (networkStream.DataAvailable)
                {
                    int length = networkStream.Read(buffer, 0, 1024);
                    int index = input.Length;
                    Array.Resize(ref input, input.Length + length);

                    Array.Copy(buffer, 0, input, index, length);
                }
            }
            return input;
        }

        private List<MemoryStream> GetMessageStreams(byte[] buffer)
        {
            var msStreams = new List<MemoryStream>();

            int bufferPosition = 0;
            //current byte position within the buffer
            byte[] bytCompleteMessage = null;

            while (bufferPosition < buffer.Length)
            {
                //locate our EOF marker.
                int intNdx = Encoding.ASCII.GetString(buffer).IndexOf(WriteBufferEof, bufferPosition, StringComparison.Ordinal);

                if (intNdx == -1)
                {
                    //The EOF marker was not found in the buffer
                    //store the buffer back into socketplus
                    //and the next time we get here we will append to it

                    int intIncompleteSize = 0;

                    if (_incompleteBuffer != null)
                    {
                        intIncompleteSize = _incompleteBuffer.Length;
                    }
                    Array.Resize(ref _incompleteBuffer, intIncompleteSize + buffer.Length - bufferPosition);
                    Array.Copy(buffer, bufferPosition, _incompleteBuffer, intIncompleteSize, buffer.Length - bufferPosition);

                    //we need to bail out of the loop since we know we don't have anymore data
                    bufferPosition = buffer.Length;
                }
                else
                {
                    if (_incompleteBuffer != null)
                    {
                        //start bytCompleteMessage array off with the incomplete buffer
                        Array.Resize(ref bytCompleteMessage, _incompleteBuffer.Length + intNdx - bufferPosition);
                        Array.Copy(_incompleteBuffer, bytCompleteMessage, _incompleteBuffer.Length);
                        Array.Copy(buffer, bufferPosition, bytCompleteMessage, _incompleteBuffer.Length,
                            intNdx - bufferPosition);
                        Array.Resize(ref _incompleteBuffer, 0);
                    }
                    else
                    {
                        Array.Resize(ref bytCompleteMessage, intNdx - bufferPosition);
                        Array.Copy(buffer, bufferPosition, bytCompleteMessage, 0, intNdx - bufferPosition);
                    }

                    if (bytCompleteMessage != null)
                    {
                        //take our complete message and move it to a new memory allocation since we are re-using bytCompleteMessage
                        byte[] newBytes = null;
                        Array.Resize(ref newBytes, bytCompleteMessage.Length);
                        Array.Copy(bytCompleteMessage, newBytes, bytCompleteMessage.Length);

                        //move the bytes to a new stream and add it to our message list
                        msStreams.Add(new MemoryStream(newBytes));
                    }

                    //if we still have data in our buffer after the EOF marker, then
                    //prime our pointer to the first byte past the EOF string.
                    bufferPosition = intNdx + WriteBufferEof.Length;
                }
            }
            return msStreams;
        }

        public async void SendMessage(NetworkMessage message)
        {
            await Task.Run(() =>
            {
                using (var networkStream = new NetworkStream(_clientSocket.Client))
                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(memoryStream, message);
                        byte[] output = memoryStream.ToArray();
                        output = AppendEofMarker(output);

                        if (networkStream.CanWrite)
                        {
                            networkStream.Write(output, 0, output.Length);
                        }
                        else
                        {
                            if (ClientDisconnect != null)
                            {
                                ClientDisconnect(this, Id);
                            }
                            Close();
                        }
                    }
                    catch (IOException)
                    {
                        if (ClientDisconnect != null)
                        {
                            ClientDisconnect(this, Id);
                        }
                        Close();
                    }
                }
            });
        }

        private static byte[] AppendEofMarker(byte[] data)
        {
            int intOriginalSize = data.Length;
            byte[] bytEof = Encoding.ASCII.GetBytes(WriteBufferEof);

            Array.Resize(ref data, data.Length + bytEof.Length);
            Array.Copy(bytEof, 0, data, intOriginalSize, bytEof.Length);

            return data;
        }

        public void Close()
        {
            _clientSocket.Close();
        }

        #region Events

        public delegate void HandleClientDisconnect(object sender, Guid id);

        public delegate void HandleMessage(object sender, NetworkMessage e);

        public event HandleMessage ClientMessage;

        public void OnClientMessage(object sender, NetworkMessage e)
        {
            var handler = ClientMessage;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event HandleClientDisconnect ClientDisconnect;

        #endregion
    }
}