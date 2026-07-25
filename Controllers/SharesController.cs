using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/shares")]
    public class SharesController : ControllerBase
    {
        private readonly ShareService _service;
        public SharesController(ShareService service) { _service = service; }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET api/shares?country=US or GET api/shares
        [HttpGet]                                  // public
        public ActionResult<List<Share>> GetAll([FromQuery] string? country)
        {
            return Ok(_service.GetAll(country));
        }

        // POST api/shares
        [HttpPost]
        [Authorize]
        public ActionResult Create([FromBody] ShareRequest req)
        {
            int id = _service.Create(CurrentUserId, req);
            return Ok(new { id });
        }

        // PUT api/shares/{id}
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Update(int id, [FromBody] ShareRequest req)
        {
            bool ok = _service.Update(id, CurrentUserId, req);
            if (!ok) return NotFound(new { message = "Share not found or not yours." });
            return Ok();
        }

        // DELETE api/shares/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            bool ok = _service.Delete(id, CurrentUserId);
            if (!ok) return NotFound(new { message = "Share not found or not yours." });
            return Ok();
        }
    }
}
