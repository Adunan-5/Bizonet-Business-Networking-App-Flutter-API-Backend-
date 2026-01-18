using Bizonet.Api.Data;
using Bizonet.Api.Helpers;
using Bizonet.Api.Models.Dtos.Auth;
using Bizonet.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Bizonet.Api.Services
{
    public interface IAuthService
    {
        Task<(bool ok, string message)> RegisterAsync(RegisterRequestDto dto);
        Task<(bool ok, string message)> VerifyOtpAsync(VerifyOtpRequestDto dto);
        Task<(bool ok, string message, LoginResponseDto? data)> LoginAsync(LoginRequestDto dto);
        Task<(bool ok, string message, LoginResponseDto? data)> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<(bool ok, string message)> LogoutAsync(LogoutRequestDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext db, IOtpService otpService, IEmailService emailService, ITokenService tokenService)
        {
            _db = db;
            _otpService = otpService;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<(bool ok, string message)> RegisterAsync(RegisterRequestDto dto)
        {
            // password regex same as your PHP
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()])(?!.*\s).{8,}$");

            if (!passwordRegex.IsMatch(dto.Password))
                return (false, "Password must be minimum 8 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character");

            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match");

            // email exists
            bool emailExists = await _db.Users.AnyAsync(x => x.Email == dto.Email && x.Active);
            if (emailExists)
                return (false, "Email already exists");

            // mobile exists
            bool mobileExists = await _db.Users.AnyAsync(x => x.MobileNo == dto.MobileNo && x.Active);
            if (mobileExists)
                return (false, "Mobile number already exists");

            // country
            var country = await _db.Countries.FirstOrDefaultAsync(x => x.CountryName == dto.CountryName);
            if (country == null)
                return (false, "Invalid CountryName");

            // state
            var state = await _db.States.FirstOrDefaultAsync(x =>
                x.StateName == dto.StateName && x.CountryId == country.CountryId);

            if (state == null)
                return (false, "Invalid StateName");

            // city
            var city = await _db.Cities.FirstOrDefaultAsync(x =>
                x.CityName == dto.CityName && x.StateId == state.StateId);

            if (city == null)
                return (false, "Invalid CityName");

            // create user
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email.Trim(),
                MobileNo = dto.MobileNo.Trim(),
                CountryCode = dto.CountryCode,
                CountryShortCode = dto.CountryShortCode,
                Address = dto.Address,
                ZipCode = dto.ZipCode,

                CountryId = country.CountryId,
                StateId = state.StateId,
                CityId = city.CityId,

                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MobileVerified = false,
                EmailVerified = false
            };

            // hash password
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // OTP row
            string otp = _otpService.GenerateOtp(6);

            var otpRow = new UserOtp
            {
                UserId = user.UserId,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                Purpose = "RegisterEmailOtp",
                CreatedAt = DateTime.UtcNow
            };

            _db.UserOtps.Add(otpRow);
            await _db.SaveChangesAsync();

            // send email
            await _emailService.SendOtpEmailAsync(user.Email!, otp, "RegisterEmailOtp");

            return (true, "User registered successfully. OTP sent to your email");
        }

        public async Task<(bool ok, string message)> VerifyOtpAsync(VerifyOtpRequestDto dto)
        {
            var email = dto.Email.Trim();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Active);
            if (user == null)
                return (false, "Invalid OTP or user does not exist");

            var otpRow = await _db.UserOtps
                .Where(x => x.UserId == user.UserId
                    && x.OtpCode == dto.Otp
                    && x.Purpose == "RegisterEmailOtp"
                    && x.IsUsed == false)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRow == null)
                return (false, "Invalid OTP");

            if (DateTime.UtcNow > otpRow.ExpiresAt)
                return (false, "OTP has expired");

            otpRow.IsUsed = true;
            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, "OTP verified successfully. Mobile number verified.");
        }

        public async Task<(bool ok, string message, LoginResponseDto? data)> LoginAsync(LoginRequestDto dto)
        {
            string input = dto.UserInput.Trim();

            // ✅ find user by email or mobile
            var user = await _db.Users.FirstOrDefaultAsync(x =>
                (x.Email == input || x.MobileNo == input) &&
                x.Active);

            if (user == null)
                return (false, "User not found or account is inactive", null);

            // ✅ if user has not verified email, restrict login
            if (!user.EmailVerified)
                return (false, "Email is not verified", null);

            // helper function - create token + save refresh token
            async Task<LoginResponseDto> GenerateLoginResponseAsync()
            {
                // ✅ Access Token
                string accessToken = _tokenService.GenerateAccessToken(user);

                // ✅ Refresh Token
                string refreshToken = _tokenService.GenerateRefreshToken();
                DateTime refreshExpiry = _tokenService.GetRefreshTokenExpiry();

                // ✅ save refresh token to DB
                var refreshRow = new UserRefreshToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    ExpiresAt = refreshExpiry,
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.UserRefreshTokens.Add(refreshRow);
                await _db.SaveChangesAsync();

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = _tokenService.GetAccessTokenExpiry(),
                    UserData = new
                    {
                        user.UserId,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.UserRole
                    }
                };
            }

            // =======================
            // ✅ METHOD: password
            // =======================
            if (dto.SignInMethod == "password")
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    return (false, "Password is required", null);

                var hasher = new PasswordHasher<User>();
                var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

                if (verify == PasswordVerificationResult.Failed)
                    return (false, "Invalid Password", null);

                var response = await GenerateLoginResponseAsync();
                return (true, "Logged in successfully.", response);
            }

            // =======================
            // ✅ METHOD: otp
            // =======================
            if (dto.SignInMethod == "otp")
            {
                string otp = _otpService.GenerateOtp(6);

                // mark previous login OTPs used
                var oldOtps = await _db.UserOtps
                    .Where(x => x.UserId == user.UserId &&
                                x.Purpose == OtpPurpose.LoginEmailOtp &&
                                x.IsUsed == false)
                    .ToListAsync();

                foreach (var old in oldOtps)
                    old.IsUsed = true;

                var otpRow = new UserOtp
                {
                    UserId = user.UserId,
                    OtpCode = otp,
                    Purpose = OtpPurpose.LoginEmailOtp,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };

                _db.UserOtps.Add(otpRow);
                await _db.SaveChangesAsync();

                if (string.IsNullOrWhiteSpace(user.Email))
                    return (false, "User email not found", null);

                // ✅ purpose controls email subject/template
                await _emailService.SendOtpEmailAsync(user.Email, otp, OtpPurpose.LoginEmailOtp);

                return (true, "OTP sent Successfully to your email", null);
            }

            // =======================
            // ✅ METHOD: otpValidation
            // =======================
            if (dto.SignInMethod == "otpValidation")
            {
                if (string.IsNullOrWhiteSpace(dto.EnteredOtp))
                    return (false, "EnteredOTP is required", null);

                var otpRow = await _db.UserOtps
                    .Where(x => x.UserId == user.UserId
                                && x.Purpose == OtpPurpose.LoginEmailOtp
                                && x.OtpCode == dto.EnteredOtp
                                && x.IsUsed == false)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRow == null)
                    return (false, "Invalid OTP", null);

                if (DateTime.UtcNow > otpRow.ExpiresAt)
                    return (false, "OTP has expired", null);

                otpRow.IsUsed = true;
                await _db.SaveChangesAsync();

                var response = await GenerateLoginResponseAsync();
                return (true, "Logged in successfully.", response);
            }

            return (false, "Invalid sign-in method", null);
        }

        public async Task<(bool ok, string message, LoginResponseDto? data)> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var refreshToken = dto.RefreshToken.Trim();

            var tokenRow = await _db.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (tokenRow == null)
                return (false, "Invalid refresh token", null);

            if (tokenRow.IsRevoked)
                return (false, "Refresh token is revoked", null);

            if (DateTime.UtcNow > tokenRow.ExpiresAt)
                return (false, "Refresh token has expired", null);

            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == tokenRow.UserId && x.Active);
            if (user == null)
                return (false, "User not found or inactive", null);

            // ✅ Rotation: revoke old refresh token
            tokenRow.IsRevoked = true;

            // ✅ issue new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRefreshRow = new UserRefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = _tokenService.GetRefreshTokenExpiry(),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.UserRefreshTokens.Add(newRefreshRow);
            await _db.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = _tokenService.GetAccessTokenExpiry(),
                UserData = new
                {
                    user.UserId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.UserRole
                }
            };

            return (true, "Token refreshed successfully.", response);
        }

        public async Task<(bool ok, string message)> LogoutAsync(LogoutRequestDto dto)
        {
            var refreshToken = dto.RefreshToken.Trim();

            var tokenRow = await _db.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (tokenRow == null)
                return (false, "Invalid refresh token");

            if (tokenRow.IsRevoked)
                return (true, "Already logged out");

            tokenRow.IsRevoked = true;
            await _db.SaveChangesAsync();

            return (true, "Logged out successfully.");
        }



    }
}
