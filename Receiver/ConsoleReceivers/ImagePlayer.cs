using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleReceivers
{
    public class ImagePlayer
    {
        private int count = 0;
        private string _authString;
        private UdpClient _udpClient;
        private Configuration _configuration;
        private string _conntectionString;
        private bool _disposed;
        private ConcurrentBag<ImageMessage> images = new();

        public void Stop()
        {
            _disposed = true;
        }
        public void Run(int index)
        {
            _configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("Configuration.json")) ?? new Configuration();


            _udpClient = new UdpClient(_configuration.Port + index + 1);


            Auth auth = new Auth();
            _authString = auth.AuthRole("RECEIVER", "RECEIVER", _configuration);
            Dns dns = new Dns();
            _conntectionString = dns.GetRegionalSender(_authString, _configuration);
            UdpClient udpClient = new UdpClient(_configuration.SendPort + index + 1);
            udpClient.Connect(_conntectionString.Split(":")[0], int.Parse(_conntectionString.Split(":")[1]));
            string json = JsonConvert.SerializeObject(new ReceiverRequest { Theme = "ion", Auth = _authString, ReplyPort = _configuration.Port + 1 + index });

            Byte[] sendBytes = Encoding.ASCII.GetBytes(json);
            udpClient.Send(sendBytes);
            udpClient.Close();


            DateTime? firstImageShown = null;
            DateTime? firstImage = null;
            string format = "yyyy-MM-ddTHH:mm:ss.fffffffK";
            DateTime now = DateTime.Now;
           
            while (!_disposed)
            {
                try
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    _udpClient.Client.ReceiveTimeout = 5000;
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = _udpClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    ImageMessage? message = JsonConvert.DeserializeObject<ImageMessage>(returnData);

                    if (message != null)
                    {
                        images.Add(message);
                    }
                    count++;
                    if (count % 10 == 0)
                    {
                        Console.WriteLine($"{index}.Received {count} messages");
                    }
                }
                catch (Exception e)
                {
                    int n = 4;
                }
                Thread.Sleep(2);
            }
        }
    }
    public class ReceiverRequest
    {
        public string Theme { get; set; }

        public int ReplyPort { get; set; }

        public string Auth { get; set; }
    }
}
