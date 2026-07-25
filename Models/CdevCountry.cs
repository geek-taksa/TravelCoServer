namespace TravelCoServer.Models
{
    // Matches the JSON shape from countries.dev
    public class CdevCountry
    {
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Capital { get; set; }
        public string Region { get; set; }
        public long Population { get; set; }
        public double Area { get; set; }
        public CdevFlags Flags { get; set; }
        public List<CdevLanguage> Languages { get; set; }
        public List<CdevCurrency> Currencies { get; set; }
    }

    public class CdevFlags 
    { 
        public string Svg { get; set; } 
    }

    public class CdevLanguage 
    { 
        public string Name { get; set; } 
    }

    public class CdevCurrency 
    { 
        public string Name { get; set; } 
    }
}
