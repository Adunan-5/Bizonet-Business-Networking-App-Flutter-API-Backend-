using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Lookups;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Services
{
    public interface ILookupsService
    {
        Task<List<CountryDto>> GetCountriesAsync();
        Task<List<StateDto>> GetStatesAsync(int countryId);
        Task<List<CityDto>> GetCitiesAsync(int stateId);
        Task<List<BusinessCategoryDto>> GetBusinessCategoriesAsync();
    }

    public class LookupsService : ILookupsService
    {
        private readonly AppDbContext _db;

        public LookupsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            return await _db.Countries
                .Where(x => x.Active == 1)
                .OrderBy(x => x.So)
                .ThenBy(x => x.CountryName)
                .Select(x => new CountryDto
                {
                    CountryId = x.CountryId,
                    CountryCode = x.CountryCode,
                    CountryName = x.CountryName,
                    PhoneCode = x.PhoneCode
                })
                .ToListAsync();
        }

        public async Task<List<StateDto>> GetStatesAsync(int countryId)
        {
            return await _db.States
                .Where(x => x.Active == 1 && x.CountryId == countryId)
                .OrderBy(x => x.So)
                .ThenBy(x => x.StateName)
                .Select(x => new StateDto
                {
                    StateId = x.StateId,
                    CountryId = x.CountryId,
                    StateName = x.StateName
                })
                .ToListAsync();
        }

        public async Task<List<CityDto>> GetCitiesAsync(int stateId)
        {
            return await _db.Cities
                .Where(x => x.Active == 1 && x.StateId == stateId)
                .OrderBy(x => x.So)
                .ThenBy(x => x.CityName)
                .Select(x => new CityDto
                {
                    CityId = x.CityId,
                    StateId = x.StateId,
                    CityName = x.CityName
                })
                .ToListAsync();
        }

        public async Task<List<BusinessCategoryDto>> GetBusinessCategoriesAsync()
        {
            return await _db.BusinessCategories
                .Where(x => x.Active == 1)
                .OrderBy(x => x.So)
                .ThenBy(x => x.CategoryName)
                .Select(x => new BusinessCategoryDto
                {
                    BusinessCategoryId = x.BusinessCategoryId,
                    CategoryName = x.CategoryName
                })
                .ToListAsync();
        }
    }
}
