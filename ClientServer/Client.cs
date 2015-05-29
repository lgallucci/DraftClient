namespace ClientServer
{
    using System;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using DraftEntities;

    public class Client
    {
        //Send Messages on Draft Picks
        public Task ServerListener;
        private UdpClient _updClient;
        protected bool IsRunning;
        private SocketClient _client;
        protected readonly Guid _clientId;
        public Client()
        {
            IsRunning = true;
            _clientId = Guid.NewGuid();
        }

        #region Network Methods
        public void ListenForServers(Action<DraftServer> serverPingCallback)
        {
            ServerListener = Task.Run(() =>
            {
                _updClient = new UdpClient
                {
                    ExclusiveAddressUse = false
                };

                var localEp = new IPEndPoint(IPAddress.Any, Server.Port);

                _updClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _updClient.ExclusiveAddressUse = false;

                _updClient.Client.Bind(localEp);

                IPAddress multicastaddress = IPAddress.Parse(Server.MulticastAddress);
                _updClient.JoinMulticastGroup(multicastaddress);

                while (IsRunning)
                {
                    var serverBroadcastData = _updClient.Receive(ref localEp);

                    var formatter = new BinaryFormatter();
                    var networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(serverBroadcastData));

                    if (networkMessage.MessageType == NetworkMessageType.BroadcastMessage && networkMessage.MessageContent is DraftServer)
                    {
                        serverPingCallback(networkMessage.MessageContent as DraftServer);
                    }
                }
            });
        }

        //Listen for Draft Messages

        //Send Messages on Draft Picks

        public void ConnectToDraftServer(string ipAddress, int port)
        {
            var _tcpClient = new TcpClient();
            _tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            _client = new SocketClient(_tcpClient, _clientId);

            _client.ClientMessage += HandleMessage;
            _client.ClientDisconnect += HandleDisconnect;

            _client.StartClient();
        }
        #endregion

        #region Event Handlers

        private void HandleMessage(object sender, NetworkMessage networkMessage)
        {
            if (networkMessage.MessageType == NetworkMessageType.LoginMessage)
            {
                if (networkMessage.MessageContent is String)
                {
                    //TODO: Handle Login
                }
            }
            else if (networkMessage.MessageType == NetworkMessageType.LogoutMessage)
            {
                if (networkMessage.MessageContent is String)
                {
                    //TODO: Handle Logout
                }
            }
            else if (networkMessage.MessageType == NetworkMessageType.PickMessage)
            {
                if (networkMessage.MessageContent is Player)
                {
                    //TODO: Handle Pick
                }
            }
        }

        private void HandleDisconnect(object sender, Guid e)
        {
            
        }

        public void SendMessage(NetworkMessageType type, object payload)
        {
            _client.SendMessage(new NetworkMessage
            {
                Id = _clientId,
                MessageType = type,
                MessageContent = payload
            });
        }

        #endregion

        public virtual void Close()
        {
            IsRunning = false;
            if (_updClient != null)
            {
                _updClient.Close();
                _updClient = null;
            }

            if (_client != null)
            {
                _client.ClientMessage -= HandleMessage;
                _client.ClientDisconnect -= HandleDisconnect;
                _client.Close();
            }

            if (ServerListener != null)
            {                
                ServerListener.Dispose();
            }
        }
    }
}
