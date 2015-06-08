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
        public Guid Id { get; set; }

        public SocketClient(TcpClient client)
        {
            _clientSocket = client;
        }

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
                    if (_networkStream.CanRead && _networkStream.DataAvailable)
                    {
                        byte[] message = await ReadMessage();
                        NetworkMessage networkMessage = null;
                        try
                        {
                            var formatter = new BinaryFormatter();
                            networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(message));
                        }
                        catch (Exception e)
                        {
                            SerializeException(this, e);
                        }

                        if (networkMessage != null && networkMessage.MessageType != NetworkMessageType.KeepAliveMessage)
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
                    var length = _networkStream.Read(buffer, 0, 1024);
                    var index = input.Length;
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
                            if (ClientDisconnect != null) ClientDisconnect(this, Id);
                            Close();
                        }
                    }
                    catch (IOException)
                    {
                        if (ClientDisconnect != null) ClientDisconnect(this, Id);
                        Close();
                    }
                    catch (Exception e)
                    {
                        SerializeException(this, e);
                    }
                }
            });
        }

        public void Close()
        {
            _networkStream.Close();
            _clientSocket.Close();
        }

        public delegate void HandleMessage(object sender, NetworkMessage e);
        public event HandleMessage ClientMessage;

        public delegate void HandleClientDisconnect(object sender, Guid id);
        public event HandleClientDisconnect ClientDisconnect;

        public delegate void HandleSerializeException(object sender, Exception e);
        public event HandleSerializeException SerializeException;
    }
}