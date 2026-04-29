using LibraryApi.DTOs;
using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        private static readonly List<RefreshToken> refreshTokens = new();

        private static readonly List<User> _users = new()
        {
            new User{Id=1, Username="efeok", Password="1234", Role="admin" },
            new User{Id=1, Username="halisgokce", Password="1234", Role="user" },
            new User{Id=1, Username="doga", Password="1234", Role="user" },
            new User{Id=1, Username="stajyerdeniz", Password="1234", Role="admin" }
        };
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { error = "Username and Password required." });

            var user = _users.FirstOrDefault(x => x.Username == request.Username && x.Password == request.Password);

            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" }); //401

            var (token, expiresAt) = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            refreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7), //7 gun geçerli
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
            });

            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                RefreshToken= refreshToken,
                Username = request.Username,
                Role = user.Role
            });
        }
        /*
Refresh Token : Access token kısa süreli olunca bir sorun çıkıyor kullanıcı her saat başı tekrar login mi olacak ? iş tam bu anda devreye refresh token girmektedir.

Access TOken > Süresi 15-60 dk arasında  Api isteklerinde kullanılır
Refresh TOken =>7-30 gün arasında  ve sadece yeni access token almak istedigimizde kullanılır...


Access token kısa oldugu icin çalınma riski az olur refresh token uzun ama sadece tek bir endpointte çalışır onu daha güvenli saklarsınız.

1=> Login
     Client => Post => login [username,password]
     Server=> {accessToken}(60dk),refreshtoken 7 gün

2 Normal kullanım 60 dakika boyunca sistemdeyiz

3 60 dakika doldu access token süresi doldu
  Server istek attığın book tarafına 401 dönecek */

        // Eski refresh token ile yeni access token alma endpointi

        [HttpPost("refresh")]
        public ActionResult<LoginResponse> Refresh([FromBody] RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { error = "Refresh token is required" });

            //1. refresh token bul
            var stored =refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

            if (stored == null)
                return Unauthorized(new { error = "Invalid refresh token" });

            //2. iptal edilmiş mi?
            if (stored.IsRevoked)
                return Unauthorized(new { error = "Refresh token has been revoked" });

            //3. süresi dolmuş mu?
            if (stored.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(new { error = "Refresh token has expired" });

            //4. Kullanıcıyı bul
            var user = _users.FirstOrDefault(x => x.Id == stored.UserId);
            if (user == null)
                return Unauthorized(new { error = "User not found" });

            //5. Eski refresh Token İptal et

            stored.IsRevoked = true;

            //6. Yeni Access Token ve Refresh Token oluştur

            var (token, expiresAt) = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            refreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });
            //7. Yeni tokenları dön
            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                Role = user.Role
            });

        }

        //Refresh token iptal eden endpoint access token süresi dolana kadar çalışmaya devam eder ama yenileme yapılmaz

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout([FromBody] LogoutRequest request)
        {
            var stored = refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
            if (stored != null)
                stored.IsRevoked = true;

            return NoContent(); //204
        }
    }
}

