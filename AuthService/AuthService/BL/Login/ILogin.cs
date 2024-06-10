namespace AuthService.BL.Login
{
    public interface ILogin
    {
        public bool LoginType(string typeName, string key);

        public bool LoginStreamer(string name, string password);
    }
}
