using Newtonsoft.Json;
using System.Text;

namespace Sender
{

    public class Auth
    {
        private static readonly HttpClient client = new HttpClient();

        public string AuthRole(string roleName, string roleKey, Configuration configuration)
        {

            var postData = new Message
            {
                TypeName = roleName,
                Key = roleKey
            };

            var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

            var response = client.PostAsync($"{configuration.AuthAddress}/LoginType", content).Result;

            string responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        private class Message 
        {
            public string TypeName { get; set; }
            public string Key { get; set; } 
        }
    }
}
