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
        HandShakeMessage = 4,
        
        //Draft Action
        PickMessage = 5,
        
        //Draft & Timer 
        DraftStartMessage = 6,
        DraftStopMessage = 7,

        //Teams
        UpdateTeamMessage = 8,

        //
        SendDraftMessage = 9,
        RetrieveDraftMessage = 10,

        SendDraftSettingsMessage = 11,
        RetrieveDraftSettingsMessage = 12
    }
}
