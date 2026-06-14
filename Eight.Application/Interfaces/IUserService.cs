using Eight.Application.DTOs.User;

namespace Eight.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(UserRequest request);
    Task SetActiveAsync(Guid id, bool isActive);
}