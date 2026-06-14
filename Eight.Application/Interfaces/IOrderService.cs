using Eight.Application.DTOs.Order;

namespace Eight.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> AddAsync(OrderRequest request);
    Task<List<OrderResponse>> GetBySessionAsync(Guid sessionId);
    Task DeleteAsync(Guid id);
}