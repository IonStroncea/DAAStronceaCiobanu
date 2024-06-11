using Newtonsoft.Json;
using System.Drawing;
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
        private int Size = 7;

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
                int y = form.Location.Y + 40;
                int width = form.Width;
                int height = form.Height-40;
                int frameWidth = width / Size;
                int frameHeight = height / Size;

               Rectangle rect = new Rectangle(x , y, width, height);
                Bitmap bigBitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format16bppRgb565);
                Graphics bigG = Graphics.FromImage(bigBitmap);
                bigG.CopyFromScreen(rect.Left, rect.Top, 0, 0, bigBitmap.Size, CopyPixelOperation.SourceCopy);
                

                List<Bitmap> images = new List<Bitmap>();
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        Rectangle m_Rectangle = new Rectangle(i* frameWidth, j* frameHeight, frameWidth, frameHeight);
                        Bitmap bmp = new Bitmap(frameWidth, frameHeight, PixelFormat.Format16bppRgb565);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.DrawImage(bigBitmap, new Rectangle(0, 0, frameWidth, frameHeight), m_Rectangle, GraphicsUnit.Pixel);
                            //g.CopyFromScreen(m_Rectangle.Left, m_Rectangle.Top, 0,0,bmp.Size, CopyPixelOperation.SourceCopy);
                        }

                        images.Add(bmp);  
                    }
                }
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        SendOneImage(images[i*Size + j], frameWidth, frameHeight, Size, i * Size + j);
                    }
                }
                lastMeasurmend = current;
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

        private void SendOneImage(Bitmap bmp, int width, int height, int size, int count)
        {
            byte[] arr = BitmapToByteArray(bmp);

            ImageMessage imageMessage = new ImageMessage {
                X = width,
                Y = height,
                Image = arr.ToList(),
                Auth = _authString,
                Theme = "ion",
                Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
                Size = size,
                Count = count
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
