using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=UrlShortenerDb;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

