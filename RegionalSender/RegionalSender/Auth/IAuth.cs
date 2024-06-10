
namespace RegionalSender.Auth
{
    public interface IAuth
    {
        public string Auth(string roleName, string roleKey, Configuration configuration);
    }
}
