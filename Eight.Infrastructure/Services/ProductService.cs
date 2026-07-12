using Eight.Application.DTOs.Product;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<List<ProductResponse>> GetByVenueAsync(Guid venueId)
        => await _db.Products
            .Where(x => x.VenueId == venueId && x.IsActive)
            .Select(x => ToResponse(x))
            .ToListAsync();

    public async Task<List<ProductResponse>> GetAllByVenueAsync(Guid venueId)
        => await _db.Products
            .Where(x => x.VenueId == venueId)
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .Select(x => ToResponse(x))
            .ToListAsync();

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            VenueId = request.VenueId
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request)
    {
        var product = await _db.Products.FindAsync(id)
            ?? throw new Exception("Məhsul tapılmadı.");
        product.Name = request.Name;
        product.Price = request.Price;
        await _db.SaveChangesAsync();
        return ToResponse(product);
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var product = await _db.Products.FindAsync(id)
            ?? throw new Exception("Məhsul tapılmadı.");
        product.IsActive = isActive;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id)
            ?? throw new Exception("Məhsul tapılmadı.");
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }

    private static ProductResponse ToResponse(Product x) =>
        new(x.Id, x.Name, x.Price, x.IsActive);


}