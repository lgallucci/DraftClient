namespace DraftEntities
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DraftTeams
    {
        public DraftTeams()
        {
        }

        public List<DraftTeam> Teams { get; set; }
    }
}
