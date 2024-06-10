﻿
using System.Web;

namespace Receiver
{
    internal class Dns
    {
        private static readonly HttpClient client = new HttpClient();

        public string GetRegionalSender(string auth, Configuration configuration)
        {
            var values = new Dictionary<string, string>
  {
      { "auth", auth }
  };
            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync($"{configuration.DnsAddress}/main/getRegionalSender", content).Result;
            string responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }
    }
}
