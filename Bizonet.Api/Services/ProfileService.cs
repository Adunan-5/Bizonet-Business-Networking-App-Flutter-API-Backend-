using Bizonet.Api.Configurations;
using Bizonet.Api.Data;
using Bizonet.Api.Models.Dtos.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Bizonet.Api.Services
{
    public interface IProfileService
    {
        Task<(bool ok, string message, ProfileResponseDto? data)> GetMyProfileAsync(int userId);
        Task<(bool ok, string message, object? data)> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
    }

    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;
        private readonly AppSettings _appSettings;

        public ProfileService(AppDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }

        public async Task<(bool ok, string message, ProfileResponseDto? data)> GetMyProfileAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.Active);
            if (user == null)
                return (false, "User not found", null);

            string? countryName = null;
            string? stateName = null;
            string? cityName = null;

            if (user.CountryId != null)
                countryName = await _db.Countries.Where(x => x.CountryId == user.CountryId).Select(x => x.CountryName).FirstOrDefaultAsync();

            if (user.StateId != null)
                stateName = await _db.States.Where(x => x.StateId == user.StateId).Select(x => x.StateName).FirstOrDefaultAsync();

            if (user.CityId != null)
                cityName = await _db.Cities.Where(x => x.CityId == user.CityId).Select(x => x.CityName).FirstOrDefaultAsync();

            // Business
            var business = await _db.BusinessInfos.FirstOrDefaultAsync(x => x.UserId == userId);

            string? businessCategoryName = null;
            string? businessCountryName = null;
            string? businessStateName = null;
            string? businessCityName = null;

            if (business != null)
            {
                if (business.BusinessCategoryId != null)
                    businessCategoryName = await _db.BusinessCategories
                        .Where(x => x.BusinessCategoryId == business.BusinessCategoryId)
                        .Select(x => x.CategoryName)
                        .FirstOrDefaultAsync();

                if (business.CountryId != null)
                    businessCountryName = await _db.Countries.Where(x => x.CountryId == business.CountryId).Select(x => x.CountryName).FirstOrDefaultAsync();

                if (business.StateId != null)
                    businessStateName = await _db.States.Where(x => x.StateId == business.StateId).Select(x => x.StateName).FirstOrDefaultAsync();

                if (business.CityId != null)
                    businessCityName = await _db.Cities.Where(x => x.CityId == business.CityId).Select(x => x.CityName).FirstOrDefaultAsync();
            }

            // ✅ Profile photo full URL
            var profilePhotoUrl = string.IsNullOrWhiteSpace(user.ProfilePhoto)
                ? ""
                : $"{_appSettings.ProfilePhotoBaseUrl}{user.ProfilePhoto}";

            var response = new ProfileResponseDto
            {
                UserData = new UserProfileDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    MobileNo = user.MobileNo,
                    MobileCountryCode = user.CountryCode,
                    MobileCountryShortCode = user.CountryShortCode,

                    Address = user.Address,
                    Designation = user.Designation,

                    CountryId = user.CountryId,
                    StateId = user.StateId,
                    CityId = user.CityId,
                    ZipCode = user.ZipCode,

                    CountryName = countryName,
                    StateName = stateName,
                    CityName = cityName,

                    ProfilePhoto = profilePhotoUrl,
                    UserRole = user.UserRole
                },

                BusinessInformation = new BusinessInfoDto
                {
                    BusinessCategoryId = business?.BusinessCategoryId,
                    BusinessCategory = businessCategoryName,

                    BusinessName = business?.CompanyName,
                    CompanyAddress = business?.CompanyAddress,
                    BusinessDescription = business?.BusinessDescription,
                    CompanyWebsite = business?.CompanyWebsite,

                    BusinessEmail = business?.BusinessEmail,
                    BusinessPhone = business?.BusinessPhone,
                    MobileCountryCode = business?.CountryCode,
                    MobileCountryShortCode = business?.CountryShortCode,

                    CountryId = business?.CountryId,
                    StateId = business?.StateId,
                    CityId = business?.CityId,
                    ZipCode = business?.ZipCode,

                    CountryName = businessCountryName,
                    StateName = businessStateName,
                    CityName = businessCityName
                }
            };

            return (true, "Profile Displayed successfully.", response);
        }

        public async Task<(bool ok, string message, object? data)> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.Active);
            if (user == null)
                return (false, "User not found", null);

            // ✅ validate file (optional)
            if (dto.ProfilePhoto != null)
            {
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(dto.ProfilePhoto.ContentType))
                    return (false, "Invalid file format. Please upload only JPG, JPEG, or PNG.", null);

                // ✅ save file
                var extension = Path.GetExtension(dto.ProfilePhoto.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";

                var folder = _appSettings.ProfilePhotoUploadPath;
                Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ProfilePhoto.CopyToAsync(stream);
                }

                user.ProfilePhoto = fileName;
            }

            // ✅ update fields only if provided
            user.UserName = dto.UserName ?? user.UserName;
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;

            user.MobileNo = dto.MobileNo ?? user.MobileNo;
            user.Address = dto.Address ?? user.Address;
            user.Designation = dto.Designation ?? user.Designation;

            user.CountryId = dto.CountryId ?? user.CountryId;
            user.StateId = dto.StateId ?? user.StateId;
            user.CityId = dto.CityId ?? user.CityId;

            user.ZipCode = dto.ZipCode ?? user.ZipCode;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // ✅ return updated details (similar to PHP)
            var profilePhotoUrl = string.IsNullOrWhiteSpace(user.ProfilePhoto)
                ? ""
                : $"{_appSettings.ProfilePhotoBaseUrl}{user.ProfilePhoto}";

            var countryName = user.CountryId != null
                ? await _db.Countries.Where(x => x.CountryId == user.CountryId).Select(x => x.CountryName).FirstOrDefaultAsync()
                : null;

            var stateName = user.StateId != null
                ? await _db.States.Where(x => x.StateId == user.StateId).Select(x => x.StateName).FirstOrDefaultAsync()
                : null;

            var cityName = user.CityId != null
                ? await _db.Cities.Where(x => x.CityId == user.CityId).Select(x => x.CityName).FirstOrDefaultAsync()
                : null;

            var updatedData = new
            {
                userId = user.UserId,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Email,
                user.MobileNo,
                user.Address,
                profilePhoto = profilePhotoUrl,
                countryName,
                stateName,
                cityName,
                user.ZipCode
            };

            return (true, "Profile updated successfully.", updatedData);
        }

    }
}
