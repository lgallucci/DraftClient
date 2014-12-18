namespace ClientServer
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using DraftEntities;

    public class SocketClient
    {
        private readonly TcpClient _clientSocket;
        private NetworkStream _networkStream;

        public SocketClient(TcpClient clientConnected)
        {
            this._clientSocket = clientConnected;
        }

        public bool Connected
        {
            get { return _clientSocket.Connected; }
        }

        public bool LoggedIn { get; set; }

        private string _clientName;

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
                            var networkMessage = (NetworkMessage) formatter.Deserialize(new MemoryStream(message));

                            HandleMessage(networkMessage);
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                    Thread.Sleep(50); // i dont want to read all the time
                }
                CleanUp();
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

        private void HandleMessage(NetworkMessage networkMessage)
        {
            if (networkMessage.MessageType == NetworkMessageType.LoginMessage)
            {
                if (networkMessage.MessageContent is String)
                {
                    OnClientLogin(this, new ClientLoginEventArgs
                    {
                        ClientName = networkMessage.MessageContent as String
                    });
                    _clientName = networkMessage.MessageContent as String;
                    LoggedIn = true;
                }
            }
            else if (networkMessage.MessageType == NetworkMessageType.LogoutMessage)
            {
                if (networkMessage.MessageContent is String)
                {
                    OnClientLogout(this, new ClientLogoutEventArgs
                    {
                        ClientName = networkMessage.MessageContent as String
                    });
                    CleanUp();
                }
            }
            else if (networkMessage.MessageType == NetworkMessageType.PickMessage)
            {
                if (networkMessage.MessageContent is Player)
                {
                    OnClientPick(this, new ClientPickEventArgs
                    {
                        Pick = networkMessage.MessageContent as Player
                    });
                }
            }
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
                        _networkStream.Write(output, 0, output.Length);
                    }
                    catch (IOException)
                    {
                        OnClientLogout(this, new ClientLogoutEventArgs());
                        this.CleanUp();
                    }

                }
            });
        }

        private void CleanUp()
        {
            LoggedIn = false;
            _networkStream.Close();
            _clientSocket.Close();
        }

        public event ClientLogin OnClientLogin;
        public event ClientLogout OnClientLogout;
        public event ClientPick OnClientPick;
    }
}