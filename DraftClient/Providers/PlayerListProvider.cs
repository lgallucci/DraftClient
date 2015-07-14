namespace DraftClient.Providers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DraftClient.ViewModel;
    using WpfControls;

    internal class PlayerListProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            List<Player> filteredPlayers = Globals.PlayerList.Players.Where(p => p.IsPicked == false && p.Name.ToLower().Contains(filter.ToLower()))
                .OrderBy(o => o.Rank).ToList();
            return filteredPlayers;
        }
    }
}