namespace ClientServer
{
    using System;

    [Serializable]
    public class NetworkMessage
    {
        public NetworkMessage()
        {
            MessageId = Guid.NewGuid();
        }

        public Guid SenderId { get; set; }

        public Guid MessageId { get; set; }

        public NetworkMessageType MessageType { get; set; }

        public object MessageContent { get; set; }
    }

    [Serializable]
    public enum NetworkMessageType
    {
        //Server discovery
        ServerBroadcast,
        //Background health
        Ackgnowledge,
        KeepAliveMessage,

        //Login/out messages
        LoginMessage,
        LogoutMessage,
        HandShakeMessage,

        //Draft Action
        PickMessage,

        //Draft & Timer 
        DraftStopMessage,
        UpdateDraftState,

        //Teams
        UpdateTeamMessage,

        //Draft Setup
        SendDraftMessage,
        RetrieveDraftMessage,
        SendDraftSettingsMessage,
        RetrieveDraftSettingsMessage
    }
}