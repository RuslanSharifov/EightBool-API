using Eight.Domain.Enums;

namespace Eight.Domain.Entities;

public class Table
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TableType Type { get; set; }
    public TableStatus Status { get; set; }
    public decimal PricePerHour { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
}
