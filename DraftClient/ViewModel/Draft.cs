namespace DraftClient.ViewModel
{
    using System.Collections.Generic;

    public class Draft
    {
        public Draft() { }

        public Draft(int rounds, int teams, int numberOfSeconds, bool server)
        {
            MaxRound = rounds;
            MaxTeam = teams;
            Picks = new List<List<DraftPick>>(rounds);
            for (int i = 0; i < rounds; i++)
            {
                Picks.Add(new List<DraftPick>(teams));
                for (int j = 0; j < teams; j++)
                {
                    Picks[i].Add(new DraftPick
                    {
                        CanEdit = server
                    });
                }
            }
            State = new DraftState(server, numberOfSeconds);
        }
        
        public int MaxRound { get; set; }
        public int MaxTeam { get; set; }
        public DraftState State { get; set; }
        public List<List<DraftPick>> Picks { get; set; }
    }
}