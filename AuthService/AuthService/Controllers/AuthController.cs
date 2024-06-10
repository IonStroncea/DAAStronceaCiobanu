using AuthService.BL;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public LoginBL _loginBL;

        public AuthController(LoginBL loginBL)
        {
            _loginBL = loginBL;
        }

        [HttpPost("/LoginType")]
        public string LoginType([FromBody]TypeParams param)
        {
            return _loginBL.LoginType(param.TypeName, param.Key);
        }

        [HttpPost("/LoginStreamer")]
        public string LoginStreamer([FromBody] StreamParams parameters)
        {
            return _loginBL.LoginStreamer(parameters.Name, parameters.Password);
        }

        [HttpGet("/Decode")]
        public string Decode(string text)
        {
            return _loginBL.Decode(text);
        }

        [HttpGet("/GenerateKeys")]
        public void GenerateKeys()
        {
            _loginBL.GenerateKeys();
        }
    }

    public class TypeParams 
    {
        public string TypeName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    public class StreamParams
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
