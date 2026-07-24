using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class CountryService
    {
        private readonly CountryRepository _repo;
        public CountryService(CountryRepository repo)   // DI injects the repo
        {
            _repo = repo;
        }

        public List<Country> GetAll()
        {
            return _repo.GetAll();
        }
    }
}
