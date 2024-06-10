using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegionalSender.Auth
{

    public class Auth : IAuth
    {
        private static readonly HttpClient client = new HttpClient();
        string IAuth.Auth(string roleName, string roleKey, Configuration configuration)
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
