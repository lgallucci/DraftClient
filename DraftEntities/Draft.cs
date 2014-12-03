namespace DraftEntities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Draft
    {
        public long pickEndTime { get; set; }
        public long pickPauseTime { get; set; }
        public int Round { get; set; }
        public int MaxRound { get; set; }
        public int Team { get; set; }
        public int MaxTeam { get; set; }
        public bool Drafting { get; set; }

        public Draft(int rounds, int teams)
        {
            MaxRound = rounds;
            MaxTeam = teams;
            pickEndTime = (DateTime.Now + new TimeSpan(0, 0, 180)).Ticks;
            pickPauseTime = -1;
        }

        public void StartDraft()
        {
            Round = 1;
            Team = 1;
            Drafting = true;
            pickEndTime = (DateTime.Now + new TimeSpan(0, 0, 180)).Ticks;
            pickPauseTime = -1;
        }

        public void PauseDraft()
        {
            pickPauseTime = DateTime.Now.Ticks;
        }

        public void ResumeDraft()
        {
            pickPauseTime = -1;
            pickEndTime = (pickEndTime - pickPauseTime) + DateTime.Now.Ticks;
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
            pickEndTime = -1;
            pickPauseTime = -1;
            Drafting = false;
        }
    }
}
