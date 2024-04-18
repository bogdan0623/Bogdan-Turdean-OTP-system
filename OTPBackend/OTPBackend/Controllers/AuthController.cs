using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using OTPBackend.Data;
using OTPBackend.DTO;
using OTPBackend.Models;
using OTPBackend.Services;

namespace OTPBackend.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : Controller
    {
        private ApplicationDbContext _db;
        private IConfiguration _configuration;

        private readonly string appName = "OTPBackend";

        public AuthController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (dto.Password != dto.PasswordConfirms)
            {
                return Unauthorized("Passwords do not match");
            }

            User user = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = HashService.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            User user = _db.Users.Where(u => u.Email == loginDto.Email).FirstOrDefault();

            if(user == null)
            {
                return Unauthorized("Invalid e-mail");
            }

            if(HashService.HashPassword(loginDto.Password) != user.Password)
            {
                return Unauthorized("Invalid password");
            }

            if(user.TfaSecret is not null)
            {
                return Ok(new
                {
                    id = user.Id,
                });
            }

            Random random = new Random();
            string secret = new(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567", 32).Select(s => s[random.Next(s.Length)]).ToArray());

            string otpAuthUrl = $"otpauth://totp/{appName}:Secret?secret={secret}&issuer={appName}".Replace(" ", "%20");

            return Ok(new
            {
                id = user.Id,
                secret = secret,
                otpauth_url = otpAuthUrl,
            });
        }

        [HttpPost("two-factor")]
        public IActionResult TwoFactor(TwoFactorDto dto)
        {
            User? user = _db.Users.Where(u => u.Id == dto.Id).FirstOrDefault();

            if(user is null)
            {
                return Unauthorized("Invalid credentials");
            }

            string secret = user.TfaSecret is not null ? user.TfaSecret : dto.Secret;

            TwoFactorAuthenticator tfa = new();
            if(!tfa.ValidateTwoFactorPIN(secret, dto.Code, true))
            {
                return Unauthorized("Invalid credentials");
            }

            if(user.TfaSecret is null)
            {
                _db.Users.Where(u => u.Email == user.Email).FirstOrDefault()!.TfaSecret = dto.Secret;
                _db.SaveChanges();
            }

                string accessToken = TokenService.CreateAccessToken(dto.Id, _configuration.GetSection("JWT:AccessKey").Value);
            string refreshToken = TokenService.CreateRefreshToken(dto.Id, _configuration.GetSection("JWT:RefreshKey").Value);

            CookieOptions cookieOptions = new();
            cookieOptions.HttpOnly = true;
            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);

            UserToken token = new()
            {
                UserId = dto.Id,
                Token = refreshToken,
                ExpiredAt = DateTime.Now.AddDays(7),
            };

            _db.UserTokens.Add(token);
            _db.SaveChanges();

            return Ok(new
            {
                token = accessToken
            });
        }

        [HttpGet("user")]
        public new IActionResult User()
        {
            string authorizationHeader = Request.Headers["Authorization"];

            if(authorizationHeader is null || authorizationHeader.Length <=8)
            {
                return Unauthorized("Unauthenticated!");
            }

            string accessToken = authorizationHeader[7..];

            int id = TokenService.DecodeToken(accessToken, out bool hasTokenExpired);

            if(hasTokenExpired)
            {
                return Unauthorized("Unauthenticated!");
            }

            User user = _db.Users.Where(u => u.Id == id).FirstOrDefault();

            if(user is null)
            {
                return Unauthorized("Unauthenticated!");
            }

            return Ok(user);
        }

        [HttpPost("refresh")]
        public IActionResult Refressh()
        {
            if (Request.Cookies["refresh_token"] is null)
            {
                return Unauthorized("Unauthenticated!");
            }

            string refreshToken = Request.Cookies["refresh_token"];

            int id = TokenService.DecodeToken(refreshToken, out bool hasTokenExpired);

            if(!_db.UserTokens.Where(u => u.UserId == id && u.Token == refreshToken && u.ExpiredAt > DateTime.Now).Any())
            {
                return Unauthorized("Unauthenticated!");
            }

            if (hasTokenExpired)
            {
                return Unauthorized("Unauthenticated! Token has expired!");
            }

            string accessToken = TokenService.CreateAccessToken(id, _configuration.GetSection("JWT:AccessKey").Value);

            return Ok(new
            {
                token = accessToken,
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            string refreshToken = Request.Cookies["refresh_token"];

            if(refreshToken is null) 
            {
                return Ok("Already Logged Out!");
            }

            _db.UserTokens.Remove(_db.UserTokens.Where(u => u.Token == refreshToken).First());
            _db.SaveChanges();

            Response.Cookies.Delete("refresh_token");

            return Ok("Logged Out Successfully!");
        }
    }
}
