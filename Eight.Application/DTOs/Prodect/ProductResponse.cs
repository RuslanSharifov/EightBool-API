namespace Eight.Application.DTOs.Product;

public record ProductResponse(Guid Id, string Name, decimal Price, bool IsActive);