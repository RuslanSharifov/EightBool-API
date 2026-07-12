using Eight.Application.DTOs.Order;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Domain.Enums;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    public async Task<OrderResponse> AddAsync(OrderRequest request)
    {
        if (request.Quantity <= 0)
            throw new Exception("Miqdar ən azı 1 olmalıdır.");

        var session = await _db.Sessions.FindAsync(request.SessionId)
            ?? throw new Exception("Sessiya tapılmadı.");

        if (session.Status == SessionStatus.Closed)
            throw new Exception("Bağlı sessiyaya sifariş əlavə edilə bilməz.");

        var product = await _db.Products.FindAsync(request.ProductId)
            ?? throw new Exception("Məhsul tapılmadı.");

        if (!product.IsActive)
            throw new Exception("Bu məhsul deaktivdir, sifariş edilə bilməz.");

        // Bu sessiyada eyni məhsul üçün artıq sətir varsa, miqdarını artır
        var existingOrder = await _db.Orders
            .FirstOrDefaultAsync(x => x.SessionId == request.SessionId && x.ProductId == request.ProductId);

        if (existingOrder is not null)
        {
            existingOrder.Quantity += request.Quantity;
            await _db.SaveChangesAsync();
            return ToResponse(existingOrder, product.Name);
        }

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
            .OrderBy(x => x.CreatedAt)
            .Select(x => ToResponse(x, x.Product.Name))
            .ToListAsync();

    public async Task DeleteAsync(Guid id)
    {
        var order = await _db.Orders
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new Exception("Sifariş tapılmadı.");

        if (order.Session.Status == SessionStatus.Closed)
            throw new Exception("Bağlı sessiyadakı sifariş silinə bilməz.");

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
    }

    private static OrderResponse ToResponse(Order x, string productName) => new(
        x.Id, productName, x.Quantity, x.UnitPrice,
        x.Quantity * x.UnitPrice, x.CreatedAt);
}