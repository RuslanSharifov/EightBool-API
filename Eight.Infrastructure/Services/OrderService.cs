using Eight.Application.DTOs.Order;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    public async Task<OrderResponse> AddAsync(OrderRequest request)
    {
        var product = await _db.Products.FindAsync(request.ProductId)
            ?? throw new Exception("Məhsul tapılmadı.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return ToResponse(order, product.Name);
    }

    public async Task<List<OrderResponse>> GetBySessionAsync(Guid sessionId)
        => await _db.Orders
            .Include(x => x.Product)
            .Where(x => x.SessionId == sessionId)
            .Select(x => ToResponse(x, x.Product.Name))
            .ToListAsync();

    public async Task DeleteAsync(Guid id)
    {
        var order = await _db.Orders.FindAsync(id)
            ?? throw new Exception("Sifariş tapılmadı.");
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
    }

    private static OrderResponse ToResponse(Order x, string productName) => new(
        x.Id, productName, x.Quantity, x.UnitPrice,
        x.Quantity * x.UnitPrice, x.CreatedAt);
}