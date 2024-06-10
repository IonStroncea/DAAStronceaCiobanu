using System.Security.Cryptography;
using System.Text;

namespace RegionalSender.Auth
{
    internal class TokenAuth : ITokenAuth
    {
        private RSACryptoServiceProvider _rsa = new();

        public bool VerifyToken(string token, string toFind)
        {
            string privatePath = "privateKey";

            try
            {
                _rsa.ImportFromPem(File.ReadAllText(privatePath).ToCharArray());
            }
            catch { }

            var bytesToDecrypt = Convert.FromBase64String(token);

            var decoded = _rsa.Decrypt(bytesToDecrypt, false);

            var result = Encoding.UTF8.GetString(decoded);

            return result.Contains(toFind);
        }
    }
}
