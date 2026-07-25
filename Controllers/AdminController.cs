using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin")]      // valid token AND role must be "admin"
    public class AdminController : ControllerBase
    {
        private readonly AdminService _service;
        public AdminController(AdminService service) { _service = service; }

        // GET api/admin/users
        [HttpGet("users")]
        public ActionResult<List<AdminUserDto>> GetUsers()
        {
            return Ok(_service.GetUsers());
        }

        // PUT api/admin/users/{id}
        [HttpPut("users/{id}")]
        public ActionResult SetUserFlags(int id, [FromBody] SetFlagsRequest req)
        {
            _service.SetUserFlags(id, req.IsLocked, req.CanShare);
            return Ok();
        }

        // GET api/admin/stats
        [HttpGet("stats")]
        public ActionResult<AdminStats> GetStats()
        {
            return Ok(_service.GetStats());
        }
    }
}
