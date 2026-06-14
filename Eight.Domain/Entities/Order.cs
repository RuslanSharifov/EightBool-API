namespace Eight.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}