namespace Eight.Application.DTOs.Session;

public record SessionRequest(
    Guid TableId,
    int CustomerCount);