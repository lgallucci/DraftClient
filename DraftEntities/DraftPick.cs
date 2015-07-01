namespace DraftEntities
{
    using System;

    [Serializable]
    public class DraftPick
    {
        public int Rank { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}