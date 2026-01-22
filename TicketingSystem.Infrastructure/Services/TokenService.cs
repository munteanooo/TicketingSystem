using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Client.Application.Contracts.DTOs;
using Client.Application.Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public TokenService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<TokenPairDto> GenerateTokensAsync(UserDto user)
        {
            // Generează Access Token
            var accessToken = GenerateJwtToken(user);

            // Generează Refresh Token
            var refreshToken = GenerateRefreshToken();

            // Salvează Refresh Token în DB
            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new TokenPairDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes()),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays()),
                User = user
            };
        }

        private string GenerateJwtToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("fullName", user.FullName),
                new Claim("role", user.Role.ToString()),
                new Claim("roleValue", ((int)user.Role).ToString())
            };

            // Pentru compatibilitate cu Authorize attribute
            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task SaveRefreshTokenAsync(Guid userId, string refreshToken)
        {
            // Șterge vechile refresh token-uri active pentru user
            var oldTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();

            foreach (var oldToken in oldTokens)
            {
                oldToken.IsActive = false;
                oldToken.RevokedAt = DateTime.UtcNow;
            }

            // Adaugă noul refresh token
            var token = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays()),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenPairDto> RefreshTokensAsync(string refreshToken)
        {
            // Găsește refresh token-ul
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == refreshToken &&
                    rt.IsActive &&
                    rt.ExpiryDate > DateTime.UtcNow);

            if (storedToken == null)
                throw new SecurityTokenException("Invalid or expired refresh token");

            // Verifică dacă userul este încă activ
            if (!storedToken.User.IsActive)
                throw new InvalidOperationException("User account is deactivated");

            // Dezactivează vechiul refresh token
            storedToken.IsActive = false;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Creează UserDto
            var userDto = new UserDto
            {
                Id = storedToken.User.Id,
                Email = storedToken.User.Email!,
                FullName = storedToken.User.FullName,
                Role = storedToken.User.Role,
                IsActive = storedToken.User.IsActive,
                PhoneNumber = storedToken.User.PhoneNumber
            };

            // Generează token-uri noi
            var tokenPair = await GenerateTokensAsync(userDto);

            return tokenPair;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT Key is missing in configuration");

            var key = Encoding.UTF8.GetBytes(keyString);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public async Task RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.IsActive = false;
                storedToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeRefreshTokensForUserAsync(Guid userId)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.IsActive = false;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserDto?> GetUserFromTokenAsync(string token)
        {
            if (!await ValidateTokenAsync(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
            var fullNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "fullName")?.Value ?? "";
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (!Enum.TryParse<UserRole>(roleClaim, out var userRole))
                userRole = UserRole.Client;

            return new UserDto
            {
                Id = userId,
                Email = emailClaim,
                FullName = fullNameClaim,
                Role = userRole,
                IsActive = true
            };
        }

        private int GetAccessTokenExpiryMinutes()
        {
            return int.TryParse(_configuration["Jwt:AccessTokenExpiryMinutes"], out var minutes)
                ? minutes : 15;
        }

        private int GetRefreshTokenExpiryDays()
        {
            return int.TryParse(_configuration["Jwt:RefreshTokenExpiryDays"], out var days)
                ? days : 7;
        }
    }
}