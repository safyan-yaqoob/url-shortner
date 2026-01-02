using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LongUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ExpiryDate);

        builder.HasIndex(x => x.ShortCode)
            .IsUnique();
    }
}

