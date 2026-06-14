namespace Eight.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid VenueId { get; set; }
    public Venue Venue { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}