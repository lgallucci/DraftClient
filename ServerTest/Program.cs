namespace ServerTest
{
    using ClientServer;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("Test Fantasy League", 12);
            Console.WriteLine(server.GetFirstIPAddress());
            server.StartServer();
        }
    }
}
