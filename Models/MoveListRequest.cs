namespace TravelCoServer.Models
{
    public class MoveListRequest
    {
        public string CountryCode { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
