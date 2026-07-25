namespace TravelCoServer.Models
{
    public class Share
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }   // from a JOIN to Countries
        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }         // username, from a JOIN to Users
        public DateTime CreatedAt { get; set; }
    }
}
