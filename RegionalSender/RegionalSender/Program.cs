namespace RegionalSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Thread t = new Thread(() => server.Run(args));

            t.Start();
            Console.ReadLine();
            server.Stop();
            t.Join();
            Console.ReadLine();
        }
    }
}
