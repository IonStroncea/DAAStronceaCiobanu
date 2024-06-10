namespace AuthService.BL.TokenGeneration
{
    public interface ITokenGenerator
    {
        public string GenerateToken(params string[] keyValues);

        public string Decode(string text);

        public void GenerateKeys();
    }
}
