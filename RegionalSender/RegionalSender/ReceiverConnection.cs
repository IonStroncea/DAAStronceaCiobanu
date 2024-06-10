using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static RegionalSender.ReceiverConnection;


namespace RegionalSender
{
    internal class ReceiverConnection
    {
        private ConcurrentQueue<ImageMessage> _queue = new ConcurrentQueue<ImageMessage>();
        private bool _disposed;
        private BlockingCollection<Receiver> _receivers = new BlockingCollection<Receiver>();
        public ProcessorConnection? ProcessorConnection { get; set; }
        private int _portListen;
        private int _portSender;
        UdpClient udpClient;

        public void Stop()
        {
            _disposed = true;
        }

        public ReceiverConnection(int portListen, int portSender)
        {
            _portListen = portListen; 
            _portSender = portSender;
            udpClient  = new UdpClient(_portSender);
        }


        public void AddMessage(ImageMessage message)
        {
            _queue.Enqueue(message);
        }

        public void AddMessages(List<ImageMessage> messages)
        {
            messages.ForEach(message => _queue.Enqueue(message));
        }

        public void RunReceivers()
        {
            while (!_disposed)
            {
                try
                {
                    ImageMessage? message = null;
                    while (_queue.TryDequeue(out message))
                    {
                        if (message == null)
                            continue;

                        string messageToSend = JsonConvert.SerializeObject(message);
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(messageToSend);

                        _receivers.Where(x => x.Theme == message.Theme).ToList().ForEach(x =>
                        {
                            try
                            {
                                udpClient.Connect(x.Address.Split(":")[0], int.Parse(x.Address.Split(":")[1]));
                                udpClient.Send(sendBytes, sendBytes.Length);
                                Console.WriteLine($"Sent message:{message.Theme} time:{message.Time}");
                            }
                            catch { }
                        });
                    }
                }
                catch { }
                Thread.Sleep(1);
            }
        }

        public void Run()
        {
            UdpClient udpClient = new UdpClient();
            while (!_disposed)
            {
                try
                {   
                    udpClient.Close();
                    udpClient = new UdpClient(_portListen);
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    udpClient.Client.ReceiveTimeout = 10000;
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    ReceiverRequest? receiverRequest = JsonConvert.DeserializeObject<ReceiverRequest>(returnData);
                    if (receiverRequest != null)
                    {
                        _receivers.Add(new Receiver { Theme = receiverRequest.Theme, Address = RemoteIpEndPoint.Address.ToString() + ":" + receiverRequest.ReplyPort });
                        if (ProcessorConnection != null)
                        {
                            ProcessorConnection.Connect(receiverRequest.Theme);
                            Console.WriteLine($"Received connect:{receiverRequest.Theme} {receiverRequest.ReplyPort}");
                        }
                    }
                }
                catch { }
                finally 
                {
                    udpClient.Close();
                }
                Thread.Sleep(1);
            }
        }
    }

    public class ReceiverRequest
    {
        public string Theme { get; set; }

        public int ReplyPort { get; set; }

        
        public string Auth { get; set; }
    }

    public class Receiver
    {
        public string Theme { get; set; }

        public string Address { get; set; }
    }
}
