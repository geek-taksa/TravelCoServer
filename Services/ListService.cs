using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class ListService
    {
        private readonly ListRepository _repo;
        public ListService(ListRepository repo) { _repo = repo; }

        public UserLists GetLists(int userId) { return _repo.GetLists(userId); }
        public void Add(int userId, string countryCode, string listType) { _repo.Add(userId, countryCode, listType); }
        public void Remove(int userId, string countryCode) { _repo.Remove(userId, countryCode); }
        public void Move(int userId, string countryCode, string toType) { _repo.Move(userId, countryCode, toType); }
    }
}
