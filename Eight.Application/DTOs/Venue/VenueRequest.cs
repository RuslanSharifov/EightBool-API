namespace Eight.Application.DTOs.Venue;

public record VenueRequest(
    string Name,
    string Address,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool ServiceChargeEnabled,
    decimal ServiceChargePercent,
    Guid AdminId);