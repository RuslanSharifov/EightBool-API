using Eight.Application.DTOs.Session;

namespace Eight.Application.Interfaces;

public interface ISessionService
{
    Task<SessionResponse> OpenAsync(SessionRequest request);
    Task<SessionResponse> CloseAsync(Guid id);
    Task<SessionResponse> GetByIdAsync(Guid id);
    Task<SessionResponse?> GetActiveByTableAsync(Guid tableId);
    Task<List<SessionResponse>> GetActiveByVenueAsync(Guid venueId);
    Task<List<SessionResponse>> GetHistoryByVenueAsync(Guid venueId, DateTime? from, DateTime? to);
    Task<SessionResponse> UpdateCustomerCountAsync(Guid sessionId, int customerCount);
}