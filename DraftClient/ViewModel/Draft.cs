namespace DraftClient.ViewModel
{
    using System;

    public class Draft
    {
        public long PickEndTime { get; set; }
        public long PickPauseTime { get; set; }
        public int Round { get; set; }
        public int MaxRound { get; set; }
        public int Team { get; set; }
        public int MaxTeam { get; set; }
        public bool Drafting { get; set; }
        public DraftPick[,] Picks { get; set; }

        public Draft(int rounds, int teams, bool server)
        {
            MaxRound = rounds;
            MaxTeam = teams;
            PickEndTime = (DateTime.Now + new TimeSpan(0, 0, 180)).Ticks;
            PickPauseTime = -1;
            Picks = new DraftPick[rounds,teams];
            for (int i = 0; i < rounds; i++)
            {
                for (int j = 0; j < teams; j++)
                {
                    Picks[i,j] = new DraftPick
                    {
                        CanEdit = server
                    };
                }
            }
        }

        public void StartDraft()
        {
            Round = 1;
            Team = 1;
            Drafting = true;
            PickEndTime = (DateTime.Now + new TimeSpan(0, 0, 180)).Ticks;
            PickPauseTime = -1;
        }

        public void PauseDraft()
        {
            PickPauseTime = DateTime.Now.Ticks;
        }

        public void ResumeDraft()
        {
            PickPauseTime = -1;
            PickEndTime = (PickEndTime - PickPauseTime) + DateTime.Now.Ticks;
        }

        public void NextPick()
        {
            Team++;
            if (Team > MaxTeam)
            {
                Team = 1;
                Round++;
            }

            if (Round > MaxRound)
            {
                EndDraft();
            }
        }

        private void EndDraft()
        {
            PickEndTime = -1;
            PickPauseTime = -1;
            Drafting = false;
        }
    }
}
