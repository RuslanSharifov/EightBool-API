namespace Eight.Application.DTOs.Session;

public record SessionResponse(
    Guid Id,
    Guid TableId,
    string TableName,
    int CustomerCount,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal TotalAmount,
    string Status);