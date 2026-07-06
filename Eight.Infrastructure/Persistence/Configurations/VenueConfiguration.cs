using Eight.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.Property(x => x.ServiceChargePercent)
               .HasPrecision(5, 2);

        builder.HasOne(v => v.Admin)
               .WithMany()
               .HasForeignKey(v => v.AdminId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}