namespace DraftClient.Controllers
{
    using System;
    using ClientServer;
    using DraftClient.View;
    using DraftEntities;
    using Omu.ValueInjecter;
    using Omu.ValueInjecter.Injections;

    public class DraftController
    {
        public Client Client { get; set; }
        public bool IsServer { get; set; }
        public ViewModel.Draft CurrentDraft { get; set; }

        private readonly MainWindow _mainWindow;

        public DraftController(Client client, MainWindow mainWindow)
        {
            Client = client;
            _mainWindow = mainWindow;
            Client.PickMade += PickMade;
            Client.RetrieveDraft += RetrieveDraft;
            Client.TeamUpdated += TeamUpdated;
            _mainWindow.Closed += RemoveHandlers;
        }
        
        #region Event Handlers

        private void RemoveHandlers(object sender, EventArgs e)
        {
            Client.PickMade -= PickMade;
            Client.RetrieveDraft -= RetrieveDraft;
            Client.TeamUpdated -= TeamUpdated;
            _mainWindow.Closed -= RemoveHandlers;
        }

        private Draft RetrieveDraft()
        {
            Mapper.AddMap<ViewModel.Draft, Draft>(src =>
            {
                var res = new Draft();
                res.InjectFrom(src);
                for (int i = 0; i < src.Picks.GetLength(0); i++)
                {
                    for (int j = 0; j < src.Picks.GetLength(1); j++)
                    {
                        res.Picks[i, j] = src.Picks[i, j].DraftedPlayer.AverageDraftPosition;
                    }
                }
                return res;
            });
            return Mapper.Map<Draft>(CurrentDraft);
        }

        private void PickMade(Player player)
        {
            throw new System.NotImplementedException();
        }

        private void TeamUpdated(DraftTeam team)
        {
            _mainWindow.UpdateTeam(Mapper.Map<ViewModel.DraftTeam>(team));
        }
        #endregion

        public void MakeMove(ViewModel.Player pick)
        {
            Client.SendMessage(NetworkMessageType.PickMessage, pick);
        }
    }
}
