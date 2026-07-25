using TravelCoServer.Models;
using TravelCoServer.Repositories;

namespace TravelCoServer.Services
{
    public class CountryService
    {
        // CONSTRUCTOR
        private readonly CountryRepository _repo;
        public CountryService(CountryRepository repo)   // DI injects the repo
        {
            _repo = repo;
        }

        // METHODS
        public List<Country> GetAll()
        {
            return _repo.GetAll();
        }

        public Country? GetByCode(string code)
        {
            return _repo.GetByCode(code);
        }
    }
}
