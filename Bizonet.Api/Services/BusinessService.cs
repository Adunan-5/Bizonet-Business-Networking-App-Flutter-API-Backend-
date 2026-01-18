using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Business;
using Bizonet.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Services
{
    public interface IBusinessService
    {
        Task<(bool ok, string message, object? data)> SaveBusinessInfoAsync(int userId, SaveBusinessInfoRequestDto dto);
    }

    public class BusinessService : IBusinessService
    {
        private readonly AppDbContext _db;

        public BusinessService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool ok, string message, object? data)> SaveBusinessInfoAsync(int userId, SaveBusinessInfoRequestDto dto)
        {
            // ✅ user must exist and active
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.Active);
            if (user == null)
                return (false, "User not found or inactive", null);

            // ✅ Lookup BusinessCategoryId (optional)
            int? businessCategoryId = null;
            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                var category = await _db.BusinessCategories
                    .FirstOrDefaultAsync(x => x.CategoryName == dto.CategoryName && x.Active == 1);

                if (category != null)
                    businessCategoryId = category.BusinessCategoryId;
            }

            // ✅ Lookup Country/State/City IDs (optional)
            int? countryId = null;
            int? stateId = null;
            int? cityId = null;

            if (!string.IsNullOrWhiteSpace(dto.CountryName))
            {
                var country = await _db.Countries.FirstOrDefaultAsync(x => x.CountryName == dto.CountryName);
                if (country != null)
                    countryId = country.CountryId;
            }

            if (!string.IsNullOrWhiteSpace(dto.StateName) && countryId != null)
            {
                var state = await _db.States.FirstOrDefaultAsync(x =>
                    x.StateName == dto.StateName && x.CountryId == countryId);

                if (state != null)
                    stateId = state.StateId;
            }

            if (!string.IsNullOrWhiteSpace(dto.CityName) && stateId != null)
            {
                var city = await _db.Cities.FirstOrDefaultAsync(x =>
                    x.CityName == dto.CityName && x.StateId == stateId);

                if (city != null)
                    cityId = city.CityId;
            }

            // ✅ Check if BusinessInfo exists for this user
            var business = await _db.BusinessInfos.FirstOrDefaultAsync(x => x.UserId == userId);

            if (business == null)
            {
                // ✅ create new
                business = new BusinessInfo
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.BusinessInfos.Add(business);
            }

            // ✅ update fields only if provided (same behavior as PHP)
            business.BusinessCategoryId = businessCategoryId ?? business.BusinessCategoryId;

            business.CompanyName = dto.CompanyName ?? business.CompanyName;
            business.CompanyAddress = dto.CompanyAddress ?? business.CompanyAddress;
            business.BusinessDescription = dto.BusinessDescription ?? business.BusinessDescription;
            business.CompanyWebsite = dto.CompanyWebsite ?? business.CompanyWebsite;

            business.BusinessEmail = dto.BusinessEmail ?? business.BusinessEmail;
            business.BusinessPhone = dto.BusinessPhone ?? business.BusinessPhone;
            business.CountryCode = dto.CountryCode ?? business.CountryCode;
            business.CountryShortCode = dto.CountryShortCode ?? business.CountryShortCode;

            business.CountryId = countryId ?? business.CountryId;
            business.StateId = stateId ?? business.StateId;
            business.CityId = cityId ?? business.CityId;

            business.ZipCode = dto.ZipCode ?? business.ZipCode;
            business.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // ✅ Response data (simple)
            var response = new
            {
                business.BusinessInfoId,
                business.BusinessCategoryId,
                dto.CategoryName, // might not match if null lookup, but ok for now
                business.CompanyName,
                business.CompanyAddress,
                business.BusinessDescription,
                business.CompanyWebsite,
                business.BusinessEmail,
                business.CountryCode,
                business.CountryShortCode,
                business.BusinessPhone,
                business.CountryId,
                business.StateId,
                business.CityId,
                business.ZipCode
            };

            return (true, "Business information saved successfully.", response);
        }
    }
}
