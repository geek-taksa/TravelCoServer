using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class ShareService
    {
        private readonly ShareRepository _repo;
        public ShareService(ShareRepository repo) { _repo = repo; }

        public List<Share> GetAll(string? countryCode) { return _repo.GetAll(countryCode); }
        public int Create(int userId, ShareRequest req) { return _repo.Create(userId, req); }

        // > 0 rows affected means it matched the user's own share
        public bool Update(int id, int userId, ShareRequest req) { return _repo.Update(id, userId, req) > 0; }
        public bool Delete(int id, int userId) { return _repo.Delete(id, userId) > 0; }
    }
}
