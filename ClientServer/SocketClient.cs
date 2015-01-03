namespace ClientServer
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void HandleMessage(object sender, NetworkMessage e);
    public delegate void ClientDisconnect(object sender, EventArgs e);

    public class SocketClient
    {
        private readonly TcpClient _clientSocket;
        private NetworkStream _networkStream;

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

                        try
                        {
                            var formatter = new BinaryFormatter();
                            var networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(message));

                            if (ClientMessage != null) ClientMessage(this, networkMessage);
                        }
                        catch (Exception)
                        {

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
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, message);
                    byte[] output = memoryStream.ToArray();

                    try
                    {
                        if (_networkStream.CanWrite)
                        {
                            _networkStream.Write(output, 0, output.Length);
                        }
                        else
                        {
                            if (ClientDisconnect != null) ClientDisconnect(this, new EventArgs());
                            Close();
                        }
                    }
                    catch (IOException)
                    {
                        if (ClientDisconnect != null) ClientDisconnect(this, new EventArgs());
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

        public event HandleMessage ClientMessage;
        public event ClientDisconnect ClientDisconnect;
    }
}