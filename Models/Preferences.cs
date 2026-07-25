namespace TravelCoServer.Models
{
    public class Preferences
    {
        public List<string> Continents { get; set; } = new List<string>();
        public List<LanguagePref> Languages { get; set; } = new List<LanguagePref>();
    }
}
