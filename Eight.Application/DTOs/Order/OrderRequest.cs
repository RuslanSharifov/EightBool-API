namespace Eight.Application.DTOs.Order;

public record OrderRequest(
    Guid SessionId,
    Guid ProductId,
    int Quantity);