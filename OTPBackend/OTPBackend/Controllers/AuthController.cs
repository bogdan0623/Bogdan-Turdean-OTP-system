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
            if(dto.FirstName == string.Empty)
            {
                return BadRequest(new { message = "First name cannot be empty!" });
            }
            if (dto.LastName == string.Empty)
            {
                return BadRequest(new { message = "Last name cannot be empty!" });
            }
            if (dto.Email == string.Empty)
            {
                return BadRequest(new { message = "Email cannot be empty!" });
            }
            else
            {
                // Validate email format
                if (!ValidateService.IsValidEmail(dto.Email))
                {
                    return BadRequest(new { message = "Insert a valid e-mail address!" });
                }
            }
            if (dto.Password == string.Empty)
            {
                return BadRequest(new { message = "Password cannot be empty!" });
            }
            if (dto.Password != dto.PasswordConfirms)
            {
                return Unauthorized(new { message = "Passwords do not match" });
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
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            if(HashService.HashPassword(loginDto.Password) != user.Password)
            {
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            return Ok(new
            {
                id = user.Id,
            });
        }

        [HttpPost("otp")]
        public IActionResult Otp(UserDto userDto)
        {
            User user = _db.Users.Where(u => u.Id == userDto.Id).FirstOrDefault();

            if (user == null)
            {
                return Unauthorized(new { message = "Unauthenticated!" });
            }

            UserOtp otpToBeDeleted = _db.UserOtps.Where(u => u.UserId == user.Id).FirstOrDefault();

            if(otpToBeDeleted is not null)
            {
                _db.UserOtps.Remove(otpToBeDeleted);
            }

            Random random = new Random();
            string secret = new(Enumerable.Repeat("0123456789", 6).Select(s => s[random.Next(s.Length)]).ToArray());

            UserOtp otp = new()
            {
                UserId = user.Id,
                Code = secret,
                ExpiredAt = DateTime.Now.AddSeconds(userDto.ExpiresIn),
            };
            _db.UserOtps.Add(otp);
            _db.SaveChanges();

            return Ok(otp);
        }

        [HttpPost("two-factor")]
        public IActionResult TwoFactor(TwoFactorDto dto)
        {
            User? user = _db.Users.Where(u => u.Id == dto.Id).FirstOrDefault();

            if(user is null)
            {
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            string secret = dto.Code;

            UserOtp otp = _db.UserOtps.Where(u => u.UserId == user.Id).FirstOrDefault();

            if(otp is null)
            {
                return Unauthorized(new { message = "Invalid code!" });
            }

            if(otp.Code != secret || otp.ExpiredAt < DateTime.Now)
            {
                return Unauthorized("Invalid code!");
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

            _db.UserOtps.Remove(_db.UserOtps.Where(u => u.Code == secret).First());
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
                return Unauthorized(new { message = "Unauthenticated!" });
            }

            string accessToken = authorizationHeader[7..];

            int id = TokenService.DecodeToken(accessToken, out bool hasTokenExpired);

            if(hasTokenExpired)
            {
                return Unauthorized(new { message = "Unauthenticated!" });
            }

            User user = _db.Users.Where(u => u.Id == id).FirstOrDefault();

            if(user is null)
            {
                return Unauthorized(new { message = "Unauthenticated!" });
            }

            return Ok(user);
        }

        [HttpPost("refresh")]
        public IActionResult Refressh()
        {
            if (Request.Cookies["refresh_token"] is null)
            {
                return Unauthorized(new { message = "Unauthenticated!" });
            }

            string refreshToken = Request.Cookies["refresh_token"];

            int id = TokenService.DecodeToken(refreshToken, out bool hasTokenExpired);

            if(!_db.UserTokens.Where(u => u.UserId == id && u.Token == refreshToken && u.ExpiredAt > DateTime.Now).Any())
            {
                return Unauthorized(new { message = "Session has expired!" });
            }

            if (hasTokenExpired)
            {
                return Unauthorized(new { message = "Unauthenticated! Token has expired!" });
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

            return Ok(new { message = "Logged out successfully!" });
        }
    }
}
