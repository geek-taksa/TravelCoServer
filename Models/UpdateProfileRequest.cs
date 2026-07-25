namespace TravelCoServer.Models
{
    public class UpdateProfileRequest // what PUT /api/users/me receives
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public Preferences Preferences { get; set; } = new Preferences();
    }
}
