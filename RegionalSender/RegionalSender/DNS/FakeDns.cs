
namespace RegionalSender.DNS
{
    internal class FakeDns : IDns
    {
        public string GetThemeAddress(string theme, string auth, Configuration configuration)
        {
            return "";
        }

        public bool RegisterDns(string role, string auth, Configuration configuration)
        {
            return true;
        }
    }
}
