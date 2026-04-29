using LibraryApi.DTOs;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryApi.Models;

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

            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                Username = request.Username,
                Role = user.Role
            });
        }
    }
}