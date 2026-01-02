using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly ApplicationDbContext _context;

    public ShortUrlRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        return await _context.ShortUrls
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, cancellationToken);
    }

    public async Task<ShortUrl> AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
    {
        await _context.ShortUrls.AddAsync(shortUrl, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return shortUrl;
    }

    public async Task<bool> ExistsAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        return await _context.ShortUrls
            .AnyAsync(x => x.ShortCode == shortCode, cancellationToken);
    }
}

