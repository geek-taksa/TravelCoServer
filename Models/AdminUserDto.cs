namespace TravelCoServer.Models
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public bool CanShare { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
