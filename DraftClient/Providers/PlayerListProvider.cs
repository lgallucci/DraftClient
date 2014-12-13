namespace DraftClient.Providers
{
    using DraftClient.ViewModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using View;
    using WpfControls;

    class PlayerListProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            List<PlayerPresentation> filteredPlayers = MainWindow.PlayerList.Players.Where(p => p.IsPicked == false && p.Name.ToLower().Contains(filter.ToLower())).OrderBy(p => p.Name).ToList();
            return filteredPlayers;
        }
    }
}
