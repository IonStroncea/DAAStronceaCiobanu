
namespace Sender
{
    internal class Dns
    {
        private static readonly HttpClient client = new HttpClient();

        public string GetImageProcessor(string auth, Configuration configuration)
        {
            var values = new Dictionary<string, string>
  {
      { "auth", auth }
  };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync($"{configuration.DnsAddress}/main/getImageProcessor",content).Result;
            string responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }
    }
}
