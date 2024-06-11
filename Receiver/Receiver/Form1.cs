namespace Receiver
{
    public partial class Form1 : Form
    {

        Thread _playerThread;
        Thread _readThread;
        ImagePlayer _player;
        List<Frame> _image;
        volatile object _imageLock = new();
        private int Size = 7;
        
        public Form1()
        { 

            InitializeComponent();

            _player = new ImagePlayer();
            _image = new List<Frame>();
            for (int i = 0; i < Size * Size; i++)
            {
                _image.Add(new Frame { Bitmap = new Bitmap(2,3) });
            }
            _playerThread = new Thread(() => _player.Run(this, _image, _imageLock, panel1));
            _readThread = new Thread(() => _player.ReceiveImage(this));
            

            _playerThread.Start();
            _readThread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _player.Stop();
            _playerThread.Join();
            _readThread.Join();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            lock (_imageLock)
            {
                Bitmap big_Bitmap = new Bitmap(_image[0].Bitmap.Width * Size, _image[0].Bitmap.Height * Size);
                using (Graphics g = Graphics.FromImage(big_Bitmap))
                {
                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            Bitmap map = _image[i * Size + j].Bitmap;
                            g.DrawImage(map, i * map.Width, j * map.Height);
                        }
                    }
                }
                e.Graphics.DrawImage(big_Bitmap, 0,0, panel1.Width, panel1.Height);
            }
            //Thread.Sleep(1);
        }
    }
}
