
namespace RegionalSender.DNS
{
    internal class Dns : IDns
    {
        private static readonly HttpClient client = new HttpClient();

        public string GetThemeAddress(string theme, string auth, Configuration configuration)
        {
            string responseString = client.PostAsync($"{configuration.DnsAddress}/main/getTranslationByName?name={theme}", null)
                .Result.Content
                .ReadAsStringAsync().Result;
            return responseString;
        }

        public bool RegisterDns(string role, string auth, Configuration configuration)
        {
            var values = new Dictionary<string, string>
            {
                { "role", role },
                { "auth", auth },
                { "port",  configuration.ReceiverListenPort.ToString()}
            };

            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync($"{configuration.DnsAddress}/main/addNewRegionalSender", content).Result;

            string responseString = response.Content.ReadAsStringAsync().Result;
            return true;
        }
    }
}
