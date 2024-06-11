
namespace RegionalSender
{
    public class Configuration
    {
        public int ProcessorPort { get; set; }
        public int ReceiverListenPort { get; set; }
        public int ReceiverSenderPort { get; set; }
        public string DnsAddress { get; set; }
        public string AuthAddress { get; set; }

        public int MaxUsers { get; set; } = 1;
    }
}
