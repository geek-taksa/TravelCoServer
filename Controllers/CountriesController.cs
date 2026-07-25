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
    }
}
