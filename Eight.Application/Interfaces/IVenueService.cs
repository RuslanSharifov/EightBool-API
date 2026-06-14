using Eight.Application.DTOs.Venue;

namespace Eight.Application.Interfaces;

public interface IVenueService
{
    Task<List<VenueResponse>> GetAllAsync();
    Task<VenueResponse> GetByIdAsync(Guid id);
    Task<VenueResponse> CreateAsync(VenueRequest request);
    Task<VenueResponse> UpdateAsync(Guid id, VenueRequest request);
    Task SetActiveAsync(Guid id, bool isActive);
}