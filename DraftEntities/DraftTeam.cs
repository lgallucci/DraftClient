namespace DraftEntities
{
    using System;

    [Serializable]
    public class DraftTeam
    {
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public int Index { get; set; }
    }
}