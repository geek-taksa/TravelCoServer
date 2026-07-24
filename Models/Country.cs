namespace TravelCoServer.Models
{
    public class Country
    {
        // properties
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Capital { get; set; }
        public string? Region { get; set; }
        public long Population { get; set; }
        public double Area { get; set; }
        public string? Flag { get; set; }
    }
}
