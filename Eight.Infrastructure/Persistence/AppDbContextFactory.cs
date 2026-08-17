using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Standart ADO.NET formatı (hiç bir format xətası verməz)
        optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.gcirheukelrlhdtqauzz;Password=EightBool_529;Timeout=300;Command Timeout=300;Pooling=true;");
        return new AppDbContext(optionsBuilder.Options);
    }
}