using Eight.Application.DTOs.User;
using Eight.Application.DTOs.Venue;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<List<UserResponse>> GetAvailableAdminsAsync();
    Task<List<UserResponse>> GetStaffByVenueAsync(Guid venueId); 
    Task<UserResponse> CreateAsync(UserRequest request);
    Task<UserResponse> GetByIdAsync(Guid id);
    Task UpdateRol(Guid id, int rol);
    Task SetActiveAsync(Guid id, bool isActive);
    Task UpdateRoleAsync(Guid id, int role);
    Task UpdateAsync(Guid id, UserUpdateRequest request);
    Task DeleteAsync(Guid id);
    Task<VenueResponse> GetUserVenue(Guid id);
    Task DeleteUserAsync(Guid id);
}