using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;
        public UserService(UserRepository repo) { _repo = repo; }

        public UserProfile? GetProfile(int userId)
        {
            User? user = _repo.GetById(userId);
            if (user == null) return null;

            return new UserProfile
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Preferences = new Preferences
                {
                    Continents = _repo.GetContinents(userId),
                    Languages = _repo.GetLanguages(userId)
                }
            };
        }

        public void UpdateProfile(int userId, UpdateProfileRequest req)
        {
            _repo.UpdateProfile(userId, req.Username, req.Email);
            if (req.Preferences != null)
            {
                _repo.SetContinents(userId, req.Preferences.Continents);
                _repo.SetLanguages(userId, req.Preferences.Languages);
            }
        }
    }
}
