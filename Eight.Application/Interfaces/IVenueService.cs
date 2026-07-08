using Eight.Application.DTOs.User;
using Eight.Application.DTOs.Venue;
using System.ComponentModel;

namespace Eight.Application.Interfaces;

public interface IVenueService
{
    Task<List<VenueResponse>> GetAllAsync();
    Task<VenueResponse> GetByIdAsync(Guid id);
    Task<VenueResponse> CreateAsync(VenueRequest request);
    Task<VenueResponse> UpdateAsync(Guid id, VenueRequest request);
    Task ChangeAdminAsync(Guid VenueId, Guid NewAdminId);
    Task DeleteAsync(Guid id);
    Task SetActiveAsync(Guid id, bool isActive);
    Task<UserResponse> GetHallAdmin(Guid VenueId);
    Task<UserResponse> UpdateHallAdminAsync(Guid venueId, Guid hallAdminId);

}