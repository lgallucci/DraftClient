namespace ServerTest
{
    using System.Threading;
    using ClientServer;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("Test Fantasy League", 12);
            
            server.StartServer();

            while (true)
            {
                Console.Clear();
                Console.WriteLine(server.GetFirstIpAddress());
                Console.WriteLine("Users Connected: " + server.Connections.Count);
                Thread.Sleep(2000);
            }
        }
    }
}
