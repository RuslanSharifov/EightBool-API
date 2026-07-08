using Eight.Application.DTOs.Session;

namespace Eight.Application.Interfaces;

public interface ISessionService
{
    Task<SessionResponse> OpenAsync(SessionRequest request);
    Task<SessionResponse> CloseAsync(Guid id);
    Task<SessionResponse> GetByIdAsync(Guid id);
    Task<List<SessionResponse>> GetActiveByVenueAsync(Guid venueId);

}