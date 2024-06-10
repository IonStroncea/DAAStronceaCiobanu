namespace Sender
{
    public partial class Form1 : Form
    {
        Thread _recorderThread;
        ScreenRecorder _recorder;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            _recorder = new ScreenRecorder();
            _recorderThread = new Thread(() => _recorder.Run(this));

            _recorderThread.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _recorder.Stop();
            _recorderThread.Join();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
