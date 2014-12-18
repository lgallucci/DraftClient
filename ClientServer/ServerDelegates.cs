namespace ClientServer
{
    using System;
    using DraftEntities;
    
    public delegate void ClientLogin(object sender, ClientLoginEventArgs e);
    public delegate void ClientLogout(object sender, ClientLogoutEventArgs e);
    public delegate void ClientPick(object sender, ClientPickEventArgs e);

    public class ClientLoginEventArgs : EventArgs
    {
        public string ClientName { get; set; }
    }

    public class ClientLogoutEventArgs : EventArgs
    {
        public string ClientName { get; set; }
    }

    public class ClientPickEventArgs : EventArgs
    {
        public Player Pick { get; set; }
    }
}
