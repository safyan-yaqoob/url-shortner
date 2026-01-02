namespace Domain.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public string LongUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
