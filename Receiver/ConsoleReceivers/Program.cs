namespace ConsoleReceivers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //int count = int.Parse(args[0]);
            int count = 2;
            List<ImagePlayer> players = new List<ImagePlayer>();
            List<Thread> playersThreads = new List<Thread>();

            for (int i = 0; i < count; i++)
            {
                ImagePlayer player = new ImagePlayer();
                players.Add(player);

                Thread t1 = new Thread(() => { player.Run(i); });

                t1.Start();
                playersThreads.Add(t1);


                Console.WriteLine("Stardet " + i);
                Thread.Sleep(5000);
            }
            Console.ReadLine();

            foreach(ImagePlayer player in players) 
            {
                player.Stop();
            }

            foreach (Thread player in playersThreads)
            {
                player.Join();
            }
        }
    }
}
