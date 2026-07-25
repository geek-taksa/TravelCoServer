using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelCoServer.Models;
using TravelCoServer.Repositories;
using TravelCoServer.Helpers;

namespace TravelCoServer.Services
{
    public class AuthService
    {
        // CONSTRUCTOR
        private readonly UserRepository _userRepo;
        private readonly IConfiguration _config;
        public AuthService(UserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        // METHODS
        public User Register(RegisterRequest req)
        {
            // reject a duplicate email
            if (_userRepo.GetByEmail(req.Email) != null)
                throw new Exception("Email already registered.");

            // hash the password (salt + hash)
            var (hash, salt) = PasswordHelper.CreateHash(req.Password);

            // build the User
            User user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            // saves it
            user.Id = _userRepo.Create(user);
            return user;
        }

        public (string token, User user) Login(LoginRequest req)
        {
            User? user = _userRepo.GetByEmail(req.Email);

            // Same generic message whether the email is unknown or the password is wrong
            if (user == null)
                throw new Exception("Invalid email or password.");
            if (user.IsLocked)
                throw new Exception("This account is locked.");
            if (!PasswordHelper.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid email or password.");

            string token = GenerateToken(user);
            return (token, user);
        }

        private string GenerateToken(User user)
        {
            // "claims" = facts baked into the token about who this is
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)  
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),   // token valid for 4 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
