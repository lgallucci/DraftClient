namespace ClientServer
{
    using System.Net.Sockets;

    public class ClientConnections
    {
        public string ConnectionName { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}