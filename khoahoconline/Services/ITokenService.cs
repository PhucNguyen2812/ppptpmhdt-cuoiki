using khoahoconline.Data.Entities;

namespace khoahoconline.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(NguoiDung nguoiDung);
        string GenerateRefreshToken();
    }
}
