namespace Sender
{
    public class ImageMessage
    {
        public string Auth { get; set; }

        public string Time { get; set; }

        public string Theme { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Size { get; set; }

        public int Count { get; set; }

        public List<byte> Image { get; set; }
    }
}
