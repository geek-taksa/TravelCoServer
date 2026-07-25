namespace TravelCoServer.Models
{
    public class User
    {
        // properties
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Role { get; set; }  // "Admin", "User"
        public bool IsLocked { get; set; }  // true if the account is locked
        public bool CanShare { get; set; }  // true if the user can share content
        public DateTime CreatedAt { get; set; }

    }
}
