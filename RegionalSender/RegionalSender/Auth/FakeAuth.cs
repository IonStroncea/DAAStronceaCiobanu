
namespace RegionalSender.Auth
{
    public class FakeAuth : IAuth
    {
        public string Auth(string roleName, string roleKey, Configuration configuration)
        {
            return "True";
        }
    }
}
