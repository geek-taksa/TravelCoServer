using Microsoft.AspNetCore.Mvc;
using TravelCoServer.Models;
using TravelCoServer.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TravelCoServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public ActionResult Register(RegisterRequest req)
        {
            try
            {
                User user = _auth.Register(req);
                return Ok(new { user.Id, user.Username, user.Email, user.Role });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST api/auth/login
        [HttpPost("login")]
        public ActionResult Login(LoginRequest req)
        {
            try
            {
                var (token, user) = _auth.Login(req);
                return Ok(new
                {
                    token,
                    user = new { user.Id, user.Username, user.Email, user.Role }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });   // HTTP 401
            }
        }

        // GET api/auth/me
        [Authorize] // requires a valid JWT in the Authorization header
        [HttpGet("me")]
        public ActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier); // gives logged-in user's Id
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { id, name, role });
        }
    }
}
