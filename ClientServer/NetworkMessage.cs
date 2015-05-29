namespace ClientServer
{
    using System;

    [Serializable]
    public class NetworkMessage
    {
        public Guid Id { get; set; }

        public NetworkMessageType MessageType { get; set; }

        public object MessageContent { get; set; }
    }

    [Serializable]
    public enum NetworkMessageType
    {
        BroadcastMessage = 0,
        KeepAliveMessage = 1,
        LoginMessage = 2,
        LogoutMessage = 3,
        PickMessage = 4,
        
        DraftStartMessage = 5,
        DraftStopMessage = 6,
        ChooseTeamMessage = 7
    }
}
