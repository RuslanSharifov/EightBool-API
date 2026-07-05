using Eight.Application.DTOs.User;

using Eight.Domain.Entities;

namespace Eight.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse> CreateAsync(UserRequest request);
    Task<UserResponse> GetByIdAsync(Guid id);
    Task UpdateRol(Guid id, int rol);
    Task SetActiveAsync(Guid id, bool isActive);
    Task UpdateRoleAsync(Guid id, int role);
    Task UpdateAsync(Guid id, UserUpdateRequest request);

    Task DeleteAsync(Guid id);
    Task<Venue> GetUserVenue(Guid id);
    

}