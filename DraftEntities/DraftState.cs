namespace DraftEntities
{
    using System;

    [Serializable]
    public class DraftState
    {
        public DateTime PickEndTime { get; set; }
        public DateTime PickPauseTime { get; set; }
        public TimeSpan PausedTime { get; set; }
        public bool Drafting { get; set; }
        public int DraftSeconds { get; set; }
    }
}
