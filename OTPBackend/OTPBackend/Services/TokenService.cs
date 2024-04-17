using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OTPBackend.Services
{
    public class TokenService
    {
        public static string CreateAccessToken(int id, string accessKey)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(accessKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(claims: claims, notBefore: DateTime.Now, expires: DateTime.Now.AddSeconds(30), 
                signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static string CreateRefreshToken(int id, string refreshKey)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(refreshKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(claims: claims, notBefore: DateTime.Now, expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static int DecodeToken(string token, out bool hasTokenExpired)
        {
            JwtSecurityToken jwtSecurityToken = new(token);
            int id = int.Parse(jwtSecurityToken.Claims.First(claim => ClaimTypes.NameIdentifier == claim.Type).Value);

            hasTokenExpired = jwtSecurityToken.ValidTo < DateTime.UtcNow;

            return id;
        }
    }
}
