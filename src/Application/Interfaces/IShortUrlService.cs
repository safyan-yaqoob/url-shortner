using Application.DTOs;

namespace Application.Interfaces;

public interface IShortUrlService
{
    Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, CancellationToken cancellationToken = default);
    Task<string?> GetLongUrlAsync(string shortCode, CancellationToken cancellationToken = default);
}

