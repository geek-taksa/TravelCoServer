using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class AdminService
    {
        private readonly AdminRepository _repo;
        public AdminService(AdminRepository repo) { _repo = repo; }

        public List<AdminUserDto> GetUsers() { return _repo.GetUsers(); }
        public void SetUserFlags(int id, bool isLocked, bool canShare) { _repo.SetUserFlags(id, isLocked, canShare); }
        public AdminStats GetStats() { return _repo.GetStats(); }
    }
}
