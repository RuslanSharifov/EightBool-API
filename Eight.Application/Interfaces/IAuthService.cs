using Eight.Application.DTOs.Auth;

namespace Eight.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}