namespace RegionalSender.DNS
{
    public interface IDns
    {
        public bool RegisterDns(string role, string auth, Configuration configuration);

        public string GetThemeAddress(string theme, string auth, Configuration configuration);
    }
}
