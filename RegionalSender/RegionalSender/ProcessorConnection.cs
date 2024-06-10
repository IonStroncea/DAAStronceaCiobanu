
using Newtonsoft.Json;
using RegionalSender.DNS;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RegionalSender
{
    internal class ProcessorConnection
    {
        private IDns _dns;
        private string _authString;
        private UdpClient _udpClient;
        private int _port;
        private ConcurrentBag<Listener> _listeners = new ConcurrentBag<Listener>();
        private volatile bool _disposed;
        private CacheSaver _cacheSaver;
        private Configuration _configuration;

        public ProcessorConnection(IDns dns, string authString, int port, CacheSaver cacheSaver,Configuration configuration)
        {
            _dns = dns;
            _authString = authString;
            
            _port = port;
            _cacheSaver = cacheSaver;
            _configuration = configuration;
        }

        public void Run() 
        {
            _udpClient = new UdpClient(_port);
            while (!_disposed)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                Byte[]? receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                string? returnData = Encoding.ASCII.GetString(receiveBytes);

                if (RemoteIpEndPoint != null && returnData != null && receiveBytes != null)
                {
                    ImageMessage? imageMessage = JsonConvert.DeserializeObject<ImageMessage>(returnData);

                    if (imageMessage != null)
                    {
                        //Console.WriteLine($"Received message:{imageMessage.Theme} time:{imageMessage.Time}");
                        _cacheSaver.AddMessage(imageMessage);
                    }
                }
                Thread.Sleep(2);
            }
        }

        public void Stop()
        {
            _disposed = true;
        }

        public void AddListener(string theme) 
        {
            if (!_listeners.Any(x => x.Theme == theme))
            {
                Connect(theme);
                _listeners.Add(new Listener { Theme = theme });
            }
        }

        public void Connect(string theme) 
        {
            UdpClient udpClient = new UdpClient(_port + 1);
            string translationAddress = _dns.GetThemeAddress(theme, _authString, _configuration);

            ConnectMessage message = new ConnectMessage { Theme = theme, Auth = _authString, ReplyPort = _port };

            string jsonString = JsonConvert.SerializeObject(message);

            Byte[] sendBytes = Encoding.ASCII.GetBytes(jsonString);
            udpClient.Connect(translationAddress.Split(':')[0], int.Parse(translationAddress.Split(':')[1]));
            udpClient.Send(sendBytes, sendBytes.Length);
            udpClient.Close();
        }
    }

    public class Listener 
    {
        public string Theme { get; set; }
    }

    public class ConnectMessage 
    {
        public string Theme { get; set; }
        public int ReplyPort { get; set; }
        public string Auth { get; set; }  
    }
}
