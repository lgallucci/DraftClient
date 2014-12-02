namespace ClientTest
{
    using ClientServer;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();  
            client.ListenForServers();
        }
    }
}
