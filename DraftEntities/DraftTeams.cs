namespace DraftEntities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DraftTeams
    {
        public DraftTeams(int numberOfTeams)
        {
            Teams = new List<DraftTeam>();

            for (int i = 0; i < numberOfTeams; i++)
            {
                Teams.Add(new DraftTeam
                {
                    Name = string.Format("Team{0}", i),
                    IsConnected = false,
                    Index = i
                });
            }
        }

        public List<DraftTeam> Teams { get; set; }
    }
}
