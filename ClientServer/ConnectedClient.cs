namespace ClientServer
{
    using System;

    public class ConnectedClient
    {
        public SocketClient Client { get; set; }

        public Guid Id
        {
            get { return Client.Id; }
            set { Client.Id = value; }
        }
    }
}