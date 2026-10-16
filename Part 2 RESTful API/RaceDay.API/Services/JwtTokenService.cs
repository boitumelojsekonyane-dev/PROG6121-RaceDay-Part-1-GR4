
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RaceDay.API.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<RaceDayUser> _userManager;

        public JwtTokenService(
            IConfiguration configuration,
            UserManager<RaceDayUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(RaceDayUser user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is missing.");

            var expiryMinutes = _configuration.GetValue<int>(
                "Jwt:ExpiryMinutes");

            if (expiryMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "JWT expiry must be greater than zero.");
            }

            var signingKey = new SymmetricSecurityKey(
                Convert.FromBase64String(jwtKey));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email));
            }

            if (user.ParticipantID.HasValue)
            {
                claims.Add(new Claim(
                    "participantId",
                    user.ParticipantID.Value.ToString()));
            }


            if (user.AdminID.HasValue)
            {
                claims.Add(new Claim(
                    "adminId",
                    user.AdminID.Value.ToString()));
            }


            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}

