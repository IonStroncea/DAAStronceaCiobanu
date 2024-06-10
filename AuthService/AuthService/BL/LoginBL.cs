using AuthService.BL.Login;
using AuthService.BL.TokenGeneration;
using System.Security.Cryptography;

namespace AuthService.BL
{
    public class LoginBL
    {
        private ILogin _login;
        private ITokenGenerator _tokenGenerator;

        public LoginBL(ILogin login, ITokenGenerator tokenGenerator)
        {
            _login = login;
            _tokenGenerator = tokenGenerator;
        }
        
        public string LoginType(string typeName, string key)
        {
            if (_login.LoginType(typeName, key))
            {
                return _tokenGenerator.GenerateToken("type", typeName);
            }
            else 
            {
                return "Error";
            }
        }
        public void GenerateKeys()
        {
            _tokenGenerator.GenerateKeys();
        }


        public string Decode(string text)
        {
            return _tokenGenerator.Decode(text);
        }

        public string LoginStreamer(string name, string password)
        {
            if (_login.LoginStreamer(name, password))
            {
                return _tokenGenerator.GenerateToken("name", name);
            }
            else
            {
                return "Error";
            }
        }
    }
}
