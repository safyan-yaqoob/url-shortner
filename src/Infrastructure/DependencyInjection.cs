using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

        return services;
    }

    public static async Task<IServiceProvider> EnsureDatabaseInitializedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.EnsureCreatedAsync();
        
        return serviceProvider;
    }
}

