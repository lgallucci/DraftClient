namespace DraftClient.Controllers
{
    using ClientServer;
    using DraftClient.ViewModel;

    public delegate void PickEventHandler(PickEventArgs e);

    public class PickEventArgs
    {
        public int AverageDraftPosition { get; set; }
    }

    public class DraftController
    {
        public Client Client { get; set; }
        public bool IsServer { get; set; }
        public Draft CurrentDraft { get; set; }

        public DraftController(Client client)
        {
            Client = client;

        }

        public void OnMove() //RECEIVE PICK FROM SERVER
        {

        }

        public void MakeMove() //SEND PICK TO SERVER
        {

        }

        public event PickEventHandler OnPickMade;
    }
}
