using Newtonsoft.Json;
using Sender;
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
        public void Run(Form form, List<Frame> image, object imageLock, Panel panel)
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


            DateTime? firstImageShown = null;
            DateTime? firstImage = null;
            string format = "yyyy-MM-ddTHH:mm:ss.fffffffK";
            DateTime now = DateTime.Now;
            Thread.Sleep(10000);
            while (!_disposed)
            {
                
                try
                {
                    List<ImageMessage> imagesToShow = new();
                    if (firstImageShown == null)
                    {
                        firstImageShown = DateTime.ParseExact(images.OrderBy(x => DateTime.ParseExact(x.Time, format, CultureInfo.InvariantCulture)).First().Time, format, CultureInfo.InvariantCulture);
                        firstImage = DateTime.Now;
                    }

                    imagesToShow = images.Where(x =>
                    {
                        DateTime imageTime = DateTime.ParseExact(x.Time, format, CultureInfo.InvariantCulture);

                        return Math.Abs(((now - firstImage) - (imageTime - firstImageShown)).Value.TotalMilliseconds) < 10;
                    }).ToList();

                    images = new ConcurrentBag<ImageMessage>(images.Except(imagesToShow).ToList());

                    lock (imageLock)
                    {
                        imagesToShow.ForEach(x =>
                        {
                            DateTime lastImage = image[x.Count].Time;
                            DateTime imageTime = DateTime.ParseExact(x.Time, format, CultureInfo.InvariantCulture);
                            if (lastImage < imageTime)
                            {
                                image[x.Count].Time = imageTime;
                                image[x.Count].Bitmap = ByteArrayToBitmap(x.Image.ToArray());
                            }

                        });
                    }

                    form.Invoke((Action)delegate { panel.Invalidate(); });
                }
                catch(Exception e)
                {
                    int n = 4;
                }
                now = DateTime.Now;
                Thread.Sleep(5);
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
