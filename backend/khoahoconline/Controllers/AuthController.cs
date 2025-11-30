using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Auth;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(ILogger<AuthController> logger, IAuthService authService, IConfiguration configuration)
        {
            _logger = logger;
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// [PUBLIC] Đăng ký tài khoản mới
        /// </summary>
        /// <param name="registerRequest">Thông tin đăng ký</param>
        /// <returns>Access token và thông tin người dùng</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            _logger.LogInformation("Controller register called");
            var result = await _authService.RegisterAsync(registerRequest);

            // Gửi refresh token trong httpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true, // JS can't access
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            });

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                NguoiDungDto = result.NguoiDungDto
            };
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Đăng ký thành công!"));
        }

        /// <summary>
        /// [PUBLIC] Đăng nhập
        /// </summary>
        /// <param name="loginRequest">Thông tin đăng nhập</param>
        /// <returns>Access token và thông tin người dùng</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Controller login called");
            var result = await _authService.LoginAsync(loginRequest);

            // gửi refreshtoken trong httpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true, // JS can't access
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            });

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                NguoiDungDto = result.NguoiDungDto
            };
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("Controller refresh token called");

            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                new UnauthorizedAccessException("Không tìm thấy refresh token");
            }

            var result = await _authService.RefreshTokenAsync(refreshToken!);

            // gửi refreshtoken trong httpOnly cookie
            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true, // JS can't access
                Secure = true, 
                SameSite = SameSiteMode.None, 
                Expires = DateTimeOffset.Now.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            });

            var response = new LoginResponse
            {
                AccessToken = result.AccessToken,
                NguoiDungDto = result.NguoiDungDto
            };
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Controller logout called");
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                new UnauthorizedAccessException("Không tìm thấy refresh token");
            }
            await _authService.LogoutAsync(refreshToken!);

            Response.Cookies.Delete("refreshToken");

            var result = ApiResponse<string>.SuccessResponse("Đăng xuất thành công.");
            return Ok(result);
        }
    }
}
