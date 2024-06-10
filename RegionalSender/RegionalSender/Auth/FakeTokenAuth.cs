
namespace RegionalSender.Auth
{
    internal class FakeTokenAuth : ITokenAuth
    {
        public bool VerifyToken(string token, string toFind)
        {
            return true;
        }
    }
}
