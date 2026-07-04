using Eight.Application.DTOs.User;

namespace Eight.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(UserRequest request);
    Task<UserResponse> GetByIdAsync(Guid id);
    Task UpdateRol(Guid id, int rol);
    Task SetActiveAsync(Guid id, bool isActive);
}