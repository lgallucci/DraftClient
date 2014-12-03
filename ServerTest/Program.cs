namespace ServerTest
{
    using ClientServer;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("Test Fantasy League", 12);
            server.StartServer();
        }
    }
}
