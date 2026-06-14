using Eight.Domain.Enums;

namespace Eight.Application.DTOs.Table;

public record TableRequest(
    string Name,
    TableType Type,
    decimal PricePerHour,
    Guid VenueId);