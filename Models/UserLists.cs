namespace TravelCoServer.Models
{
    public class UserLists
    {
        public List<Country> Visited { get; set; } = new List<Country>();
        public List<Country> Wishlist { get; set; } = new List<Country>();
    }
}
