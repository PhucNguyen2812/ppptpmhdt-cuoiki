using khoahoconline.Dtos.Auth;

namespace khoahoconline.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string request);
        Task LogoutAsync(string request);
    }
}
