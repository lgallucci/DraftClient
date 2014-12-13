namespace DraftEntities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Player
    {
        public int AverageDraftPosition { get; set; }
        public string Name { get; set; }
        public PlayerPosition Position { get; set; }
        public string Team { get; set; }
        public int ByeWeek { get; set; }
        public int YahooAverageDraftPosition { get; set; }
        public int ESPNAverageDraftPosition { get; set; }
        public int CBSAverageDraftPosition { get; set; }
        public decimal ProjectedPoints { get; set; }
    }
}
