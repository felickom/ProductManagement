using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.API.Data;

namespace ProductManagement.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(Apiuser user)
        {
            try
            {
                var securityKey = GetSecurityKey();
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = CreateClaims(user);
                var tokenExpiration = DateTime.Now.AddHours(GetTokenExpirationHours());

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: tokenExpiration,
                    signingCredentials: credentials
                );

                _logger.LogInformation("Generated token for user {Username} expiring at {Expiration}",
                    user.Username, tokenExpiration);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {Username}", user.Username);
                throw;
            }
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSecurityKey();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return false;
            }
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("JWT key is missing in configuration");
                throw new InvalidOperationException("JWT key is not configured");
            }

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        private Claim[] CreateClaims(Apiuser user)
        {
            return new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ClientId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };
        }

        private int GetTokenExpirationHours()
        {
            if (int.TryParse(_configuration["Jwt:ExpirationHours"], out int hours) && hours > 0)
            {
                return hours;
            }

            return 3; // Default expiration of 3 hours
        }
    }
}