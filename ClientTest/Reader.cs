namespace ClientTest
{
    using System;
    using System.Threading;

    internal class Reader
    {
        private static Thread inputThread;
        private static readonly AutoResetEvent getInput;
        private static readonly AutoResetEvent gotInput;
        private static string input;

        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
            {
                return input;
            }
            throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}