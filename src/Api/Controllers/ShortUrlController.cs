using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;

    public ShortUrlController(IShortUrlService shortUrlService)
    {
        _shortUrlService = shortUrlService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateShortUrlResponse>> CreateShortUrl([FromBody] CreateShortUrlRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _shortUrlService.CreateShortUrlAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{shortCode}")]
    public async Task<ActionResult> RedirectToLongUrl(string shortCode, CancellationToken cancellationToken)
    {
        var longUrl = await _shortUrlService.GetLongUrlAsync(shortCode, cancellationToken);
        
        if (string.IsNullOrEmpty(longUrl))
        {
            return NotFound();
        }

        return Ok(longUrl);
    }
}

