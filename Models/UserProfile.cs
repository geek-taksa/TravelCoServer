namespace TravelCoServer.Models
{
    public class UserProfile // what GET /api/users/me returns
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Preferences Preferences { get; set; } = new Preferences();
    }
}
