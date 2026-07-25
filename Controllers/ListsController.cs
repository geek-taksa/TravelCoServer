using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/lists")]
    [Authorize]   // every action requires a valid token
    public class ListsController : ControllerBase
    {
        private readonly ListService _service;
        public ListsController(ListService service) { _service = service; }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET api/lists
        [HttpGet]
        public ActionResult<UserLists> GetLists()
        {
            return Ok(_service.GetLists(CurrentUserId));
        }

        // POST api/lists/{type}
        [HttpPost("{type}")]
        public ActionResult Add(string type, [FromBody] AddToListRequest req)
        {
            _service.Add(CurrentUserId, req.CountryCode, type);
            return Ok();
        }

        // DELETE api/lists/{type}/{countryCode}
        [HttpDelete("{type}/{countryCode}")]
        public ActionResult Remove(string type, string countryCode)
        {
            _service.Remove(CurrentUserId, countryCode);
            return Ok();
        }

        // PUT api/lists/move
        [HttpPut("move")]
        public ActionResult Move([FromBody] MoveListRequest req)
        {
            _service.Move(CurrentUserId, req.CountryCode, req.To);
            return Ok();
        }
    }
}
