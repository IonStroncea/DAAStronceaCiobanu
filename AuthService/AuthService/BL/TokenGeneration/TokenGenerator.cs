using System.Security.Cryptography;
using System.Text;

namespace AuthService.BL.TokenGeneration
{
    public class TokenGenerator : ITokenGenerator
    {

        private RSACryptoServiceProvider _rsa = new();

        IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public string Decode(string text)
        {
            //string publicPath = _configuration["PublicKey"] ?? "";
            string privatePath = _configuration["PrivateKey"] ?? "";

            //_rsa.ImportFromPem(File.ReadAllText(publicPath).ToCharArray());
            _rsa.ImportFromPem(File.ReadAllText(privatePath).ToCharArray());
            //string key = File.ReadAllText(privatePath).Replace("44", "");
            //_rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(key), out int _);

            var bytesToDecrypt = Convert.FromBase64String(text);

            var decoded = _rsa.Decrypt(bytesToDecrypt, false);

            var result = Encoding.UTF8.GetString(decoded);

            return result;
        }

        public void GenerateKeys()
        {
            _rsa = new RSACryptoServiceProvider(2048);

            string publicPem = _rsa.ExportRSAPublicKeyPem();
            string privatePen = _rsa.ExportPkcs8PrivateKeyPem();

            string publicPath = _configuration["PublicKey"] ?? "";
            string privatePath = _configuration["PrivateKey"] ?? "";

            File.WriteAllText(publicPath, publicPem);
            File.WriteAllText(privatePath, privatePen);
        }

        public string GenerateToken(params string[] keyValues)
        {
            string publicPath = _configuration["PublicKey"] ?? "";
            //string privatePath = _configuration["PrivateKey"] ?? "";

            //_rsa.ImportFromPem(File.ReadAllText(privatePath).ToCharArray());
            _rsa.ImportFromPem(File.ReadAllText(publicPath).ToCharArray());

            if (keyValues.Length % 2 != 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            for (int i = 0; i < keyValues.Length; i += 2)
            {
                sb.Append($"\"{keyValues[i]}\"");
                sb.Append(":");
                sb.Append($"\"{keyValues[i + 1]}\"");
                sb.Append( i + 2 < keyValues.Length ? ";" : "");
            }

            sb.Append("}");

            var bytesToEncrypt = Encoding.UTF8.GetBytes(sb.ToString());
            var encryptedData = _rsa.Encrypt(bytesToEncrypt, false);
            var base64Encrypted = Convert.ToBase64String(encryptedData);

            return base64Encrypted;
        }
    }
}
