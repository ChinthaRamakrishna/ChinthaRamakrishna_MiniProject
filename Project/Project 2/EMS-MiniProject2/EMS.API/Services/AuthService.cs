using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EMS.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext  _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db     = db;
            _config = config;
        }

        // ── Register ──────────────────────────────────────────────────────────
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var exists = await _db.AppUsers
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (exists)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already exists. Please choose another."
                };

            var user = new AppUser
            {
                Username     = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role         = string.IsNullOrWhiteSpace(dto.Role) ? "Viewer" : dto.Role,
                CreatedAt    = DateTime.UtcNow
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success  = true,
                Message  = "Account created successfully.",
                Username = user.Username,
                Role     = user.Role
            };
        }

        // ── Login ─────────────────────────────────────────────────────────────
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _db.AppUsers
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password."
                };

            var token = GenerateToken(user);

            return new AuthResponseDto
            {
                Success  = true,
                Message  = "Login successful.",
                Username = user.Username,
                Role     = user.Role,
                Token    = token
            };
        }

        // ── Token generation ──────────────────────────────────────────────────
        private string GenerateToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username),
                new Claim(ClaimTypes.Role,           user.Role)
            };

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:             _config["Jwt:Issuer"],
                audience:           _config["Jwt:Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(
                                        double.Parse(_config["Jwt:ExpiryHours"] ?? "8")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
