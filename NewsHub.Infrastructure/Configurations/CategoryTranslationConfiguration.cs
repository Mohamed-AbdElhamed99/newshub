using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class CategoryTranslationConfiguration : IEntityTypeConfiguration<CategoryTranslation>
{
    public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);

        builder.HasIndex(t => new { t.CategoryId, t.LanguageCode }).IsUnique();
        builder.HasIndex(t => new { t.LanguageCode, t.Slug }).IsUnique();
    }
}