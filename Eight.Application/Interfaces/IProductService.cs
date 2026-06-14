using Eight.Application.DTOs.Product;

namespace Eight.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetByVenueAsync(Guid venueId);
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request);
    Task SetActiveAsync(Guid id, bool isActive);
    Task DeleteAsync(Guid id);
}