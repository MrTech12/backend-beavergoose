using AccountMicroservice.DTOs;
using Common.Configuration.Helpers;
using Common.Configuration.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccountMicroservice.Helpers
{
    public class TokenHelper
    {
        private readonly IConfigHelper _configHelper;

        public TokenHelper(IConfigHelper configHelper) 
        {
            this._configHelper = configHelper;
        }
        public string CreateAccessToken(UserDTO userDto)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.UserId),
                new Claim("Username", userDto.UserName),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configHelper.GetConfigValue("JWT", "Secret")));
            var authIssuer = this._configHelper.GetConfigValue("JWT", "Issuer");
            var signCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: authIssuer,
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: signCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configHelper.GetConfigValue("JWT", "Secret")));
            var authIssuer = this._configHelper.GetConfigValue("JWT", "Issuer");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authSigningKey,
                ValidateIssuer = true,
                ValidIssuer = authIssuer,
                ValidateAudience = false,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("The received access token is not valid.");
            }
            return principal;
        }
    }
}
