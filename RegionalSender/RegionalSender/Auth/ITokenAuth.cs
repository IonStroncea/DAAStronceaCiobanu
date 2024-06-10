namespace RegionalSender.Auth
{
    internal interface ITokenAuth
    {
        public bool VerifyToken(string token, string toFind);
    }
}
