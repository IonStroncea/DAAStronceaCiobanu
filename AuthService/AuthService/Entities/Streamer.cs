namespace AuthService.Entities
{
    public class Streamer : BaseEntity
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
