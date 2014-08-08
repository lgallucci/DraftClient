using DraftClient.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls;

namespace DraftClient.Providers
{
    class PlayerListProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            List<PlayerPresentation> filteredPlayers = MainWindow.playerList.Players.Where(p => p.IsPicked == false && p.Name.ToLower().Contains(filter.ToLower())).OrderBy(p => p.Name).ToList();
            return filteredPlayers;
        }
    }


}
