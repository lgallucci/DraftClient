namespace ClientServer
{
    using System;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
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

        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();
            WaitForRequest();
        }

        public void WaitForRequest()
        {
            byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];

            _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = _clientSocket.GetStream();

            //do the job with the data here
            var formatter = new BinaryFormatter();
            var networkMessage = (NetworkMessage)formatter.Deserialize(networkStream);

            HandleMessage(networkMessage);

            WaitForRequest();
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

        public void SendMessage(NetworkMessage message)
        {
            var formatter = new BinaryFormatter();
            using (NetworkStream networkStream = _clientSocket.GetStream())
            {
                formatter.Serialize(networkStream, message);
                networkStream.Flush();
            }
        }

        public event ClientLogin OnClientLogin;
        public event ClientLogout OnClientLogout;
        public event ClientPick OnClientPick;
    }
}