using Microsoft.AspNetCore.Mvc;
using TravelCoServer.Models;
using TravelCoServer.Services;
using Microsoft.AspNetCore.Authorization;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _service;
        private readonly CountryImportService _importService;
        public CountriesController(CountryService service, CountryImportService importService)   
        {
            _service = service;
            _importService = importService;
        }

        // GET api/countries
        [HttpGet]
        public ActionResult<List<Country>> GetAll(
        [FromQuery] string? search, [FromQuery] string? region,
        [FromQuery] string? language, [FromQuery] string? currency,
        [FromQuery] string? sort, [FromQuery] string? order)
        {
            return Ok(_service.GetCountries(search, region, language, currency, sort, order));
        }

        // GET api/countries/{code}
        [HttpGet("{code}")]
        public ActionResult<Country> GetByCode(string code)
        {
            var country = _service.GetByCode(code);
            if (country == null)
                return NotFound();     // returns HTTP 404
            return Ok(country);        // returns HTTP 200 + the country
        }

        // FOR ADMINS ONLY:

        // POST api/countries
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create([FromBody] Country c)
        {
            _service.Create(c);
            return Ok();
        }

        // PUT api/countries/{code}
        [HttpPut("{code}")]
        [Authorize(Roles = "admin")]
        public ActionResult Update(string code, [FromBody] Country c)
        {
            c.Code = code;                 // trust the code from the URL, not the body
            bool ok = _service.Update(c);
            if (!ok) return NotFound();
            return Ok();
        }

        // DELETE api/countries/{code}
        [HttpDelete("{code}")]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(string code)
        {
            bool ok = _service.Delete(code);
            if (!ok) return NotFound();
            return Ok();
        }

        //For importing countries from the REST Countries API (Admin only)
        // POST api/countries/import
        [HttpPost("import")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Import()
        {
            int count = await _importService.ImportAsync();
            return Ok(new { imported = count });
        }

        //For getting the counts of countries by region (for homepage)
        // GET api/countries/region-counts
        [HttpGet("region-counts")]
        public ActionResult<Dictionary<string, int>> RegionCounts()
        {
            return Ok(_service.GetRegionCounts());
        }

        // For sorting functionality on the countries page (sort by currency or language)

        // GET api/countries/languages
        [HttpGet("languages")]
        public ActionResult<List<string>> Languages() { return Ok(_service.GetLanguages()); }

        // GET api/countries/currencies
        [HttpGet("currencies")]
        public ActionResult<List<string>> Currencies() { return Ok(_service.GetCurrencies()); }
    }
}
