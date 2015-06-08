namespace DraftClient.Controllers
{
    using System;
    using System.Threading;
    using ClientServer;

    public sealed class ConnectionServer
    {
        private AutoResetEvent _connectionReset;
        public Client Connection { get; set; }
        
        private static readonly ConnectionServer instance = new ConnectionServer();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConnectionServer()
        {
        }

        private ConnectionServer()
        {
            Connection = new Client();
            Connection.ServerHandshake += ServerHandshake;
        }

        public static ConnectionServer Instance
        {
            get
            {
                return instance;
            }
        }

        public void StartServer(string leagueName, int numberOfTeams)
        {
            Connection.ServerHandshake -= ServerHandshake;
            Connection = new Server(leagueName, numberOfTeams);
            ((Server)Connection).StartServer();
        }

        public void ResetConnection()
        {
            Connection.ServerHandshake -= ServerHandshake;
            Connection = new Client();
            Connection.ServerHandshake += ServerHandshake;
        }

        public void ConnectToDraft(string ipAddress, int ipPort)
        {
            Connection.ConnectToDraftServer(ipAddress, ipPort);

            _connectionReset = new AutoResetEvent(false);
            Connection.SendMessage(NetworkMessageType.LoginMessage, Connection.ClientId.ToString());

            if (!_connectionReset.WaitOne(5000))
            {
                throw new TimeoutException("Couldn't connect to draft server");
            }
        }

        private void ServerHandshake()
        {
            _connectionReset.Set();
        }

    }
}
