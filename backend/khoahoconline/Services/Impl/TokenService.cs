using Microsoft.IdentityModel.Tokens;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

            // Get ALL roles from NguoiDungVaiTros relationship
            var roles = nguoiDung.NguoiDungVaiTros?
                .Where(ndvt => ndvt.IdVaiTroNavigation != null)
                .Select(ndvt => ndvt.IdVaiTroNavigation!.TenVaiTro)
                .ToList() ?? new List<string>();

            // If no roles found, default to HOCVIEN
            if (roles.Count == 0)
            {
                roles.Add("HOCVIEN");
            }

            _logger.LogInformation($"ten vai tro: {string.Join(", ", roles)}");

            // Build claims list with all roles
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.Email!)
            };

            // Add all roles as separate Role claims (ASP.NET Core supports multiple role claims)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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