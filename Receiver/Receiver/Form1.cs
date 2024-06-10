namespace Receiver
{
    public partial class Form1 : Form
    {

        Thread _playerThread;
        Thread _readThread;
        ImagePlayer _player;
        Bitmap _image;
        volatile object _imageLock = new();
        
        public Form1()
        { 

            InitializeComponent();

            _player = new ImagePlayer();
            _image = new Bitmap(panel1.Width, panel1.Height);
            _playerThread = new Thread(() => _player.Run(this, ref _image, _imageLock, panel1));
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
                e.Graphics.DrawImage(_image, Point.Empty);
            }
            Thread.Sleep(10);
        }
    }
}
