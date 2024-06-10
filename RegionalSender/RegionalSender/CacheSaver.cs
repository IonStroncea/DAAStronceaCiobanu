using System.Collections.Concurrent;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RegionalSender
{
    internal class CacheSaver
    {
        private ConcurrentQueue<ImageMessage> _queue = new ConcurrentQueue<ImageMessage> ();
        private ConcurrentBag<ImageMessage> _list = new ConcurrentBag<ImageMessage>();
        private ReceiverConnection _connection;
        private bool _disposed;
        private double _lastTime = 30 * 1000;
        private double _lastSendTime = 15 * 1000;

        public CacheSaver(ReceiverConnection connection)
        {
            _connection = connection;
        }

        public void Stop()
        {
            _disposed = true;
        }
        

        public void AddMessage(ImageMessage message)
        {
            //Console.WriteLine($"Send to cache saver:{message.Theme} {message.Time}");
            _queue.Enqueue (message);
        }

        public void Run()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            //IDatabase db = redis.GetDatabase();

            while (!_disposed)
            {
                while (_queue.TryDequeue(out var message))
                {
                    if (message == null)
                    {
                        continue;
                    }

                    //string? jsonList = db.StringGet(message.Theme);
                    List<ImageMessage>? list = _list.ToList();

/*                    if (jsonList != null)
                    {
                        list = JsonConvert.DeserializeObject<List<ImageMessage>>(jsonList);
                    }
*/
                    if (list != null)
                    {
                        list.Add(message);
                        //Console.WriteLine($"Saved message:{message.Theme} time:{message.Time}");
                    }
                    else
                    {
                        list = new List<ImageMessage> { message };
                        Console.WriteLine($"Created list. Saved message:{message.Theme} time:{message.Time}");
                    }

                    string format = "yyyy-MM-ddTHH:mm:ss.fffffffK";
                    list = list.OrderBy(x => DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture)).ToList();
                    DateTime now = DateTime.Now;

                    if (list.Any(x => DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture).AddMilliseconds(_lastTime) > now))
                    {
                        List<ImageMessage> messagesToSend = list.Where(x => DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture).AddMilliseconds(_lastSendTime) < now).ToList();

                        messagesToSend.ForEach(x => list.Remove(x));
                        _connection.AddMessages(messagesToSend);
                    }
                    _list = new ConcurrentBag<ImageMessage>(list);
                    //jsonList = JsonConvert.SerializeObject(list);
                    //db.StringSet(message.Theme, jsonList);
                    Thread.Sleep(1);
                }
            }
        }
    }
}
