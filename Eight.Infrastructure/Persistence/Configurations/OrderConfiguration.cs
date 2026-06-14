using Eight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eight.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Ignore(x => x.TotalPrice);

        builder.HasOne(x => x.Session)
               .WithMany(x => x.Orders)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}