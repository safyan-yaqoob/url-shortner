using Domain.Entities;

namespace Domain.Interfaces;

public interface IShortUrlRepository
{
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
    Task<ShortUrl> AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string shortCode, CancellationToken cancellationToken = default);
}

