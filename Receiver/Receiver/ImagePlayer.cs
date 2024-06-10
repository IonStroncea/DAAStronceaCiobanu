using Newtonsoft.Json;
using Sender;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Receiver
{
    public class ImagePlayer
    {
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
        public void Run(Form form, ref Bitmap image, object imageLock, Panel panel)
        {
            _configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("Configuration.json")) ?? new Configuration();


            _udpClient = new UdpClient(_configuration.Port);


            Auth auth = new Auth();
            _authString = auth.AuthRole("RECEIVER", "RECEIVER", _configuration);
            Dns dns = new Dns();
            _conntectionString = dns.GetRegionalSender(_authString, _configuration);
            UdpClient udpClient = new UdpClient(_configuration.SendPort);
            udpClient.Connect(_conntectionString.Split(":")[0], int.Parse(_conntectionString.Split(":")[1]));
            string json = JsonConvert.SerializeObject(new ReceiverRequest { Theme = "ion", Auth = _authString, ReplyPort = _configuration.Port });

            Byte[] sendBytes = Encoding.ASCII.GetBytes(json);
            udpClient.Send(sendBytes);
            udpClient.Close();


            DateTime? lastShown = null;
            DateTime? lastShownImageTime = null;
            string format = "yyyy-MM-ddTHH:mm:ss.fffffffK";
            while (!_disposed)
            {
                DateTime now = DateTime.Now;
                try
                {
                    ImageMessage? message = new ImageMessage();
                    if (lastShown == null)
                    {
                        message = images.ToList().OrderBy(x => DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture)).First();
                        lastShownImageTime = DateTime.ParseExact(message.Time, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        message = images.ToList().OrderBy(x => DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture)).Where(x => Math.Abs((DateTime.ParseExact(x.Time, format, System.Globalization.CultureInfo.InvariantCulture) - lastShownImageTime).Value.TotalMilliseconds - (now-lastShown).Value.TotalMilliseconds) < 6).First();
                        lastShownImageTime = DateTime.ParseExact(message.Time, format, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (message != null)
                    {
                        
                        var image1 = ByteArrayToBitmap(message.Image.ToArray());
                        lock (imageLock)
                        {
                            image = image1;
                            form.Invoke((Action)delegate { panel.Invalidate(); });
                        }
                    }

                    lastShown = now;
                }
                catch(Exception e)
                {
                    int n = 4;
                }
                
                Thread.Sleep(2);
            }
        }

        public static Bitmap ByteArrayToBitmap(byte[] image)
        {
            byte[] imageData;
            Bitmap bmp;
            using (var ms = new MemoryStream(image))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        public void ReceiveImage(Form form)
        {
            Thread.Sleep(5000);
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

                    if(message != null) 
                    {
                        images.Add(message);
                    }
                }
                catch { }
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
