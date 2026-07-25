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

        public void Create(Country c) 
        {
            _repo.Create(c);
        }
        public bool Update(Country c) 
        {
            return _repo.Update(c) > 0; 
        }
        public bool Delete(string code) 
        { 
            return _repo.Delete(code) > 0;
        }
        public Dictionary<string, int> GetRegionCounts() 
        { 
            return _repo.GetRegionCounts(); 
        }
        public List<Country> GetCountries(string? search, string? region, string? language, string? currency, string? sort, string? order)
        {
            return _repo.GetCountries(search, region, language, currency, sort, order);
        }
    }
}
