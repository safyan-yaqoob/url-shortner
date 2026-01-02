using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class ShortUrlService : IShortUrlService
{
    private readonly IShortUrlRepository _repository;
    private readonly IMemoryCache _cache;
    private const int MaxRetryAttempts = 10;
    private const int ShortCodeLength = 8;
    private const string CacheKeyPrefix = "short_";

    public ShortUrlService(IShortUrlRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (!UrlValidator.IsValidUrl(request.LongUrl))
        {
            throw new ArgumentException("Invalid URL format", nameof(request.LongUrl));
        }

        var shortCode = await GenerateUniqueShortCodeAsync(cancellationToken);

        var expiryDate = DateTime.UtcNow.AddHours(24);

        var shortUrl = new ShortUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = request.LongUrl,
            ShortCode = shortCode,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = expiryDate
        };

        await _repository.AddAsync(shortUrl, cancellationToken);

        var cacheKey = GetCacheKey(shortCode);
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiryDate
        };

        cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            if (reason == EvictionReason.Expired)
            {
            }
        });

        _cache.Set(cacheKey, shortUrl, cacheOptions);

        return new CreateShortUrlResponse
        {
            ShortCode = shortCode,
            ShortUrl = $"/{shortCode}"
        };
    }

    public async Task<string?> GetLongUrlAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(shortCode);
        
        if (_cache.TryGetValue(cacheKey, out ShortUrl? cachedShortUrl))
        {
            if (cachedShortUrl != null && !IsExpired(cachedShortUrl))
            {
                return cachedShortUrl.LongUrl;
            }
            _cache.Remove(cacheKey);
        }

        var shortUrl = await _repository.GetByShortCodeAsync(shortCode, cancellationToken);
        
        if (shortUrl == null)
        {
            return null;
        }

        if (IsExpired(shortUrl))
        {
            _cache.Remove(cacheKey);
            return null;
        }

        var cacheOptions = new MemoryCacheEntryOptions();
        if (shortUrl.ExpiryDate.HasValue)
        {
            cacheOptions.AbsoluteExpiration = shortUrl.ExpiryDate.Value;
        }
        else
        {
            cacheOptions.AbsoluteExpiration = DateTime.UtcNow.AddHours(24);
        }

        _cache.Set(cacheKey, shortUrl, cacheOptions);

        return shortUrl.LongUrl;
    }

    private async Task<string> GenerateUniqueShortCodeAsync(CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            var shortCode = GenerateRandomShortCode();
            
            var exists = await _repository.ExistsAsync(shortCode, cancellationToken);
            if (!exists)
            {
                return shortCode;
            }
        }

        throw new InvalidOperationException("Unable to generate unique short code after multiple attempts");
    }

    private string GenerateRandomShortCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, ShortCodeLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private bool IsExpired(ShortUrl shortUrl)
    {
        return shortUrl.ExpiryDate.HasValue && shortUrl.ExpiryDate.Value <= DateTime.UtcNow;
    }

    private string GetCacheKey(string shortCode)
    {
        return $"{CacheKeyPrefix}{shortCode}";
    }
}

