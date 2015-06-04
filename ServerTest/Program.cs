namespace ServerTest
{
    using System.Threading;
    using ClientServer;
    using System;
    using System.Linq;

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
                Console.WriteLine("Users Connected: " + server.Connections.Count(c => c.Client.Connected));
                Console.WriteLine("Users:" + server.Connections.Select(c => c.Id.ToString()).DefaultIfEmpty().Aggregate((a, x) => a + ", " + x));
                Thread.Sleep(2000);
            }
        }
    }
}
