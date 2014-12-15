namespace DraftEntities
{
    using System;

    [Serializable]
    public class DraftServer
    {
        public string FantasyDraft { get; set; }
        public int ConnectedPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string IpAddress { get; set; }
        public int IpPort { get; set; }
    }
}
