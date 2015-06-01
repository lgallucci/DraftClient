namespace DraftClient.Controllers
{
    using ClientServer;
    using DraftClient.ViewModel;
    using DraftEntities;

    public class DraftController
    {
        public Client Client { get; set; }
        public bool IsServer { get; set; }
        public Draft CurrentDraft { get; set; }

        public DraftController(Client client)
        {
            Client = client;
        }

        public void OnMove() //TODO:RECEIVE PICK FROM SERVER
        {
            //OnPickMade(new PickEventArgs() /*{ AverageDraftPosition = }*/);
        }

        public void MakeMove(Player pick) //TODO:SEND PICK TO SERVER
        {
            Client.SendMessage(NetworkMessageType.PickMessage, pick);
        }
    }
}
