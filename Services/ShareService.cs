using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class ShareService
    {
        private readonly ShareRepository _repo;
        private readonly UserRepository _userRepo;
        public ShareService(ShareRepository repo, UserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public int Create(int userId, ShareRequest req)
        {
            User? user = _userRepo.GetById(userId);
            if (user == null || !user.CanShare)
                throw new Exception("You are blocked from sharing.");
            return _repo.Create(userId, req);
        }


        public List<Share> GetAll(string? countryCode) { return _repo.GetAll(countryCode); }

        // > 0 rows affected means it matched the user's own share
        public bool Update(int id, int userId, ShareRequest req) { return _repo.Update(id, userId, req) > 0; }
        public bool Delete(int id, int userId) { return _repo.Delete(id, userId) > 0; }
    }
}
