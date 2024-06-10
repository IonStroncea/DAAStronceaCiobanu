using AuthService.DAL;

namespace AuthService.BL.Login
{
    public class Login : ILogin
    {
        public AuthContext _context;

        public Login(AuthContext context)
        {
            _context = context;
        }

        public bool LoginStreamer(string name, string password)
        {
            var streamer = _context.Streamers.FirstOrDefault(x => x.Login == name && x.Password == password);
            
            return streamer != null;
        }

        public bool LoginType(string typeName, string key)
        {
            var role = _context.Roles.FirstOrDefault(x => x.RoleName == typeName && x.RoleKey == key);

            return role != null;
        }
    }
}
