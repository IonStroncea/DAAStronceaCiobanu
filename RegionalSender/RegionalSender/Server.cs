using Newtonsoft.Json;
using RegionalSender.Auth;
using RegionalSender.DNS;

namespace RegionalSender
{
    public class Server
    {
        private volatile IAuth _auth = new RegionalSender.Auth.Auth();
        private volatile IDns _dns = new Dns();
        private volatile string _authString = string.Empty;
        private volatile ITokenAuth _tokenAuth = new TokenAuth();
        private Thread _cacheThread;
        private Thread _receiverThread;
        private Thread _receiverThreadSend;
        private Thread _processorThread;
        private CacheSaver _cacheSaver;
        private ProcessorConnection _processorConnection;
        private ReceiverConnection _receiverConnection;
        private Configuration _configuration;

        public void Run(string[] args)
        {
            string jsonConfig = File.ReadAllText("Configuration.json");
            _configuration = JsonConvert.DeserializeObject< Configuration >(jsonConfig) ?? new Configuration();

            if (args.Length == 3)
            {
                _configuration.ProcessorPort = int.Parse(args[0]);
                _configuration.ReceiverListenPort = int.Parse(args[1]);
                _configuration.ReceiverSenderPort = int.Parse(args[2]);
            }

            string roleName = "REGIONAL_SENDER";
            string roleKey = "REGIONAL_SENDER";

            int i = 0;
            bool success = false;

            while (i < 3)
            {
                try
                {
                    _authString = _auth.Auth(roleName, roleKey, _configuration);

                    success = _dns.RegisterDns(roleName, _authString, _configuration);
                    if (success)
                    {
                        i = 4;
                        break;
                    }
                    i++;
                }
                catch (Exception) 
                {
                    i++;
                }
            }

            if (!success)
            {
                return;
            }

            _receiverConnection = new ReceiverConnection(_configuration.ReceiverListenPort, _configuration.ReceiverSenderPort);
            _cacheSaver = new CacheSaver(_receiverConnection);
            _processorConnection = new ProcessorConnection(_dns, _authString, _configuration.ProcessorPort, _cacheSaver, _configuration);
            _receiverConnection.ProcessorConnection = _processorConnection;

            Replicator replicator = new Replicator(_configuration);

            _cacheThread = new Thread(() => { _cacheSaver.Run(); });
            _receiverThread = new Thread(() => { _receiverConnection.Run(replicator, _configuration.MaxUsers); });
            _processorThread = new Thread(() => { _processorConnection.Run(); });
            _receiverThreadSend = new Thread(() => { _receiverConnection.RunReceivers(); });

            _receiverThreadSend.Start();
            _cacheThread.Start();
            _receiverThread.Start();
            _processorThread.Start();

            _cacheThread.Join();
            _receiverThread.Join();
            _processorThread.Join();
            _receiverThreadSend.Join();
        }

        public void Stop()
        {
            
            _processorConnection.Stop();
            _cacheSaver.Stop();
            _receiverConnection.Stop();
        }
    }
}
