namespace Eight.Application.DTOs.Venue;

public record VenueResponse(
    Guid Id,
    string Name,
    Guid AdminID,
    string Address,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive,
    bool ServiceChargeEnabled,
    decimal ServiceChargePercent);