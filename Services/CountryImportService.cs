using System.Text.Json;
using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class CountryImportService
    {
        private readonly HttpClient _http;
        private readonly CountryRepository _repo;
        public CountryImportService(HttpClient http, CountryRepository repo)
        {
            _http = http;
            _repo = repo;
        }

        public async Task<int> ImportAsync()
        {
            // 1. fetch the whole list from countries.dev as JSON text
            string json = await _http.GetStringAsync("https://countries.dev/countries");

            // 2. deserialize — case-insensitive so camelCase JSON maps to our PascalCase DTOs
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<CdevCountry> countries = JsonSerializer.Deserialize<List<CdevCountry>>(json, options);

            int count = 0;
            foreach (CdevCountry cc in countries)
            {
                if (string.IsNullOrEmpty(cc.Alpha2Code)) continue;   // skip anything without a code

                // 3. map the DTO to our own Country model
                Country c = new Country
                {
                    Code = cc.Alpha2Code,
                    Name = cc.Name,
                    Capital = cc.Capital,
                    Region = cc.Region,
                    Population = cc.Population,
                    Area = cc.Area,
                    Flag = cc.Flags?.Svg          // ?. guards against a missing flags object
                };

                // 4. upsert the country
                _repo.Upsert(c);

                // 5. refresh its languages
                _repo.ClearLanguages(c.Code);
                if (cc.Languages != null)
                    foreach (var lang in cc.Languages)
                        if (!string.IsNullOrEmpty(lang.Name))
                            _repo.AddLanguage(c.Code, lang.Name);

                // 6. refresh its currencies
                _repo.ClearCurrencies(c.Code);
                if (cc.Currencies != null)
                    foreach (var cur in cc.Currencies)
                        if (!string.IsNullOrEmpty(cur.Name))
                            _repo.AddCurrency(c.Code, cur.Name);

                count++;
            }
            return count;
        }
    }
}
