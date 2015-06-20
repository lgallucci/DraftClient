namespace DraftClient.ViewModel
{
    using System;

    public class Draft
    {
        public Draft(int rounds, int teams, int numberOfSeconds, bool server)
        {
            MaxRound = rounds;
            MaxTeam = teams;
            Picks = new DraftPick[rounds, teams];
            for (int i = 0; i < rounds; i++)
            {
                for (int j = 0; j < teams; j++)
                {
                    Picks[i, j] = new DraftPick
                    {
                        CanEdit = server
                    };
                }
            }
            State = new DraftState(server, numberOfSeconds);
        }

        public int Round { get; set; }
        public int MaxRound { get; set; }
        public int Team { get; set; }
        public int MaxTeam { get; set; }
        public DraftState State { get; set; }
        public DraftPick[,] Picks { get; set; }

        public void NextPick()
        {
            Team++;
            if (Team > MaxTeam)
            {
                Team = 1;
                Round++;
            }
        }
    }
}