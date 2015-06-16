namespace ClientServer
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketClient
    {
        private readonly TcpClient _clientSocket;
        private NetworkStream _networkStream;
        //TODO: Incomplete buffer
        //TODO: End of file marker
        //TODO: Read multiple messages in one stream

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
            _networkStream = _clientSocket.GetStream();
            ListenForMessages();
        }
        private void ListenForMessages()
        {
            Task.Run(async () =>
            {
                while (Connected)
                {
                    if (_clientSocket.Available > 0)
                    {
                        //TODO: NEW READ METHOD WITH EOF AND NEW NETWORKSTREAM AND EVERYTHING
                    }
                    if (_networkStream.CanRead && _networkStream.DataAvailable)
                    {
                        byte[] message = await ReadMessage();
                        NetworkMessage networkMessage = null;

                        var formatter = new BinaryFormatter();
                        networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(message));

                        if (networkMessage.MessageType != NetworkMessageType.KeepAliveMessage)
                        {
                            Task.Run(() => ClientMessage(this, networkMessage));
                        }
                    }
                    Thread.Sleep(50); // i dont want to read all the time
                }
                Close();
            });
        }

        private async Task<byte[]> ReadMessage()
        {
            return await Task.Run(() =>
            {
                var buffer = new byte[1024];
                var input = new byte[0];
                while (_networkStream.DataAvailable)
                {
                    int length = _networkStream.Read(buffer, 0, 1024);
                    int index = input.Length;
                    Array.Resize(ref input, input.Length + length);

                    Array.Copy(buffer, 0, input, index, length);
                }
                return input;
            });
        }

        public async void SendMessage(NetworkMessage message)
        {
            await Task.Run(() =>
            {
                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(memoryStream, message);
                        byte[] output = memoryStream.ToArray();


                        if (_networkStream.CanWrite)
                        {
                            _networkStream.Write(output, 0, output.Length);
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

        public void Close()
        {
            _networkStream.Close();
            _clientSocket.Close();
        }

        #region Events

        public delegate void HandleClientDisconnect(object sender, Guid id);

        public delegate void HandleMessage(object sender, NetworkMessage e);

        public event HandleMessage ClientMessage;

        public event HandleClientDisconnect ClientDisconnect;

        #endregion
    }
}