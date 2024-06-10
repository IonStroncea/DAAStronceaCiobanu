using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace Sender
{
    public class ScreenRecorder
    {
        private volatile bool _disposed;
        private volatile int _framesPerSecond = 20;
        private volatile string _authString;
        private volatile UdpClient _udpClient;
        private volatile Configuration _configuration;
        private volatile string _conntectionString;

        public void Run(Form form)
        {
            
            DateTime lastMeasurmend = DateTime.Now;
            _configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("Configuration.json")) ?? new Configuration();
            _udpClient = new UdpClient(_configuration.Port);
            Auth auth = new Auth();
            _authString = auth.AuthRole("SENDER", "SENDER", _configuration);
            Dns dns = new Dns();
            _conntectionString = dns.GetImageProcessor(_authString, _configuration);
            
            while (!_disposed)
            {
                DateTime current = DateTime.Now;

                TimeSpan toSleep = current - lastMeasurmend;
                if (TimeSpan.FromMilliseconds(1000/20) - toSleep > TimeSpan.Zero) 
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000 / 20) - toSleep);
                }

                int x = form.Location.X;
                int y = form.Location.Y;
                int width = form.Width;
                int height = form.Height;

                Rectangle rect = new Rectangle(x, y+10, width, height-10);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format16bppRgb565);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

                SendOneImage(bmp, width, height-10);
            }
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {

                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            };
        }

        private void SendOneImage(Bitmap bmp, int width, int height)
        {
            byte[] arr = BitmapToByteArray(bmp);

            ImageMessage imageMessage = new ImageMessage {
                X = width,
                Y = height,
                Image = arr.ToList(),
                Auth = _authString,
                Theme = "ion",
                Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
            };

            string jsonString = JsonConvert.SerializeObject(imageMessage);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(jsonString);
            _udpClient.Connect(_conntectionString.Split(":")[0], int.Parse(_conntectionString.Split(":")[1]));
            _udpClient.Send(sendBytes, sendBytes.Length);
        }

        public void Stop()
        {
            _disposed = true;
        }
    }
}
