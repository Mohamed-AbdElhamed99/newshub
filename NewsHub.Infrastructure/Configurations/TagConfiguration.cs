using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasMany(t => t.Translations)
            .WithOne()
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TagTranslationConfiguration : IEntityTypeConfiguration<TagTranslation>
{
    public void Configure(EntityTypeBuilder<TagTranslation> builder)
    {
        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.LanguageCode).HasMaxLength(10).IsRequired();
        builder.Property(tt => tt.Name).HasMaxLength(150).IsRequired();
        builder.Property(tt => tt.Slug).HasMaxLength(150).IsRequired();

        builder.HasIndex(tt => new { tt.TagId, tt.LanguageCode }).IsUnique();
        builder.HasIndex(tt => new { tt.LanguageCode, tt.Slug }).IsUnique();
    }
}