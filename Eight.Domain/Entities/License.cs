namespace Eight.Domain.Entities;

public class License
{
    public Guid Id { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
}