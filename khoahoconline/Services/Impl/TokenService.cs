using Microsoft.IdentityModel.Tokens;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace khoahoconline.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(NguoiDung nguoiDung)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            _logger.LogInformation($" id = {nguoiDung.Id.ToString()}");
            _logger.LogInformation($"ten dang nhap: {nguoiDung.Email}");

            // Get role from NguoiDungVaiTros relationship
            var vaiTroNavigation = nguoiDung.NguoiDungVaiTros?.FirstOrDefault()?.IdVaiTroNavigation;
            var tenVaiTro = vaiTroNavigation?.TenVaiTro ?? "USER";

            _logger.LogInformation($"ten vai tro: {tenVaiTro}");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.Email!),
                new Claim(ClaimTypes.Role, tenVaiTro)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                signingCredentials: cred
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) // use 'using' to ensure proper disposal because RandomNumberGenerator implements IDisposable
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}