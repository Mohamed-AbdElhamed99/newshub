using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class ArticleTranslationConfiguration : IEntityTypeConfiguration<ArticleTranslation>
{
    public void Configure(EntityTypeBuilder<ArticleTranslation> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Content).IsRequired();
        builder.Property(t => t.Excerpt).HasMaxLength(500);
        builder.Property(t => t.MetaTitle).HasMaxLength(160);
        builder.Property(t => t.MetaDescription).HasMaxLength(300);

        // one translation per article per language
        builder.HasIndex(t => new { t.ArticleId, t.LanguageCode }).IsUnique();

        // slug must be unique per language across articles, for clean routing
        builder.HasIndex(t => new { t.LanguageCode, t.Slug }).IsUnique();
    }
}