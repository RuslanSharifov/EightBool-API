namespace Eight.Domain.Entities;

public class Venue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ServiceChargeEnabled { get; set; } = false;
    public decimal ServiceChargePercent { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public License? License { get; set; }

    // Relations
    public Guid AdminId { get; set; }
    public User Admin { get; set; } = null!;
    public ICollection<Table> Tables { get; set; } = new List<Table>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
