using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.HasKey(at => new { at.ArticleId, at.TagId });

        builder.HasOne<Article>()
            .WithMany(a => a.Tags)
            .HasForeignKey(at => at.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(at => at.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}