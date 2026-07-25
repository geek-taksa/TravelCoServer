namespace TravelCoServer.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Prompt { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
