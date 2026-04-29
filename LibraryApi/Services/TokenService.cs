using LibraryApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Claim = System.Security.Claims.Claim;
using System.Security.Cryptography;


namespace LibraryApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            // Token içerisine yazacağımız bilgileri (Claims)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
                };

            // İmza için kullanılacak gizli anahtarı hazırlayalım

            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            //Token Bitiş Zamanı
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

            //Token Oluştur
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            //string çevir
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expiresAt);
          }
        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
            //Bu rastgele 64 byte üretip Base64 stringe çeviriyor. Refresh tokenlar genellikle uzun ve tahmin edilemez stringlerdir.
            //Bu şekilde güvenli bir refresh token oluruz.
        }
        }
}
