using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelCoServer.Models;
using TravelCoServer.Services;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        public UsersController(UserService service) { _service = service; }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET api/user/me
        [HttpGet("me")]
        public ActionResult<UserProfile> Me()
        {
            UserProfile? profile = _service.GetProfile(CurrentUserId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        // PUT api/user/me
        [HttpPut("me")]
        public ActionResult UpdateMe([FromBody] UpdateProfileRequest req)
        {
            _service.UpdateProfile(CurrentUserId, req);
            return Ok();
        }
    }
}
