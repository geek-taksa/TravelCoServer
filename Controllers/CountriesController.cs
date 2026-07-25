using Microsoft.AspNetCore.Mvc;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _service;
        public CountriesController(CountryService service)   
        {
            _service = service;
        }

        // GET api/countries
        [HttpGet]
        public ActionResult<List<Country>> GetAll()
        {
            return Ok(_service.GetAll());
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
    }
}
