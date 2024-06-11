
namespace RegionalSender
{
    public class Replicator
    {
        int ProcessorPort;
        int ReceiverListenPort;
        int ReceiverSenderPort;

        public Replicator(string[] args)
        {
            if (args.Length == 3)
            {
                ProcessorPort = int.Parse(args[0]);
                ReceiverListenPort = int.Parse(args[1]);
                ReceiverSenderPort = int.Parse(args[2]);
            }
        }

        public Replicator(Configuration configuration)
        {
            ProcessorPort = configuration.ProcessorPort;
            ReceiverListenPort = configuration.ReceiverListenPort;
            ReceiverSenderPort = configuration.ReceiverSenderPort;
        }

        public void Replicate()
        {
            ProcessorPort++;
            ReceiverListenPort++;
            ReceiverSenderPort++;

            string strCmdText;
            strCmdText = $"/C RegionalSender.exe {ProcessorPort} {ReceiverListenPort} {ReceiverSenderPort}";
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
        }
    }
}
