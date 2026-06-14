using Eight.Domain.Enums;

namespace Eight.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public int CustomerCount { get; set; }
    public decimal TotalAmount { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Open;

    public Guid TableId { get; set; }
    public Table Table { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}