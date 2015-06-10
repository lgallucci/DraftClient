namespace DraftClient.Providers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DraftClient.View;
    using DraftClient.ViewModel;
    using WpfControls;

    internal class PlayerListProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            List<Player> filteredPlayers = MainWindow.PlayerList.Players.Where(p => p.IsPicked == false && p.Name.ToLower().Contains(filter.ToLower())).OrderBy(p => p.Name).ToList();
            return filteredPlayers;
        }
    }
}