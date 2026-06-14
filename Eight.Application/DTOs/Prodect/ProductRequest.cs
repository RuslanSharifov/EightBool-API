namespace Eight.Application.DTOs.Product;

public record ProductRequest(
    string Name, 
    decimal Price, 
    Guid VenueId
);