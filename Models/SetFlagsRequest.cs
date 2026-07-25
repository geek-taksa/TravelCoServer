namespace TravelCoServer.Models
{
    public class SetFlagsRequest
    {
        public bool IsLocked { get; set; }
        public bool CanShare { get; set; }
    }
}
