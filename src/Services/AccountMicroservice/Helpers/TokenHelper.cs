using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountMicroservice.Helpers
{
    public class TokenHelper
    {
        public Dictionary<string, string> CreateToken(IdentityUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("Username", user.UserName),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveConfigHelper.GetConfigValue("JWT", "Secret")));
            var authIssuer = RetrieveConfigHelper.GetConfigValue("JWT", "Issuer");
            var expireDate = RetrieveConfigHelper.GetConfigValue("JWT", "ExpirationInDays");
            var signCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: authIssuer,
                claims: authClaims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(expireDate)),
                signingCredentials: signCredentials);

            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new Dictionary<string, string>() { { returnToken, user.Id.ToString() } };
        }
    }
}
