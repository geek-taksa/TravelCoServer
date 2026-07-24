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
        public CountriesController(CountryService service)   // DI injects the service
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<Country>> GetAll()
        {
            return Ok(_service.GetAll());
        }
    }
}
