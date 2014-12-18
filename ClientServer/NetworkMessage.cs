namespace ClientServer
{
    using System;

    [Serializable]
    public class NetworkMessage
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public NetworkMessageType MessageType { get; set; }

        public object MessageContent { get; set; }
    }

    [Serializable]
    public enum NetworkMessageType
    {
        BroadcastMessage = 0,
        LoginMessage = 1,
        LogoutMessage = 2,
        PickMessage = 3,
        
        DraftStartMessage = 4,
        DraftStopMessage = 5,

        KeepAliveMessage = 6
    }
}
