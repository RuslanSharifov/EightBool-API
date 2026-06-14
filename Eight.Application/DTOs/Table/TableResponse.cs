using Eight.Domain.Enums;

namespace Eight.Application.DTOs.Table;

public record TableResponse(
    Guid Id,
    string Name,
    TableType Type,
    TableStatus Status,
    decimal PricePerHour,
    bool IsActive);