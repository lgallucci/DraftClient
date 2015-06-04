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
        //Background health
        BroadcastMessage = 0,
        KeepAliveMessage = 1,
        LoginMessage = 2,
        LogoutMessage = 3,
        
        //Draft Action
        PickMessage = 4,
        
        //Draft & Timer 
        DraftStartMessage = 5,
        DraftStopMessage = 6,

        //Teams
        UpdateTeamMessage = 7,
        RetrieveDraftMessage = 8
    }
}
