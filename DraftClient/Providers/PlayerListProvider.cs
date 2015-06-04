namespace DraftClient.Providers
{
    using DraftClient.ViewModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using View;
    using ViewModel;
    using WpfControls;

    class PlayerListProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            List<Player> filteredPlayers = MainWindow.PlayerList.Players.Where(p => p.IsPicked == false && p.Name.ToLower().Contains(filter.ToLower())).OrderBy(p => p.Name).ToList();
            return filteredPlayers;
        }
    }
}
