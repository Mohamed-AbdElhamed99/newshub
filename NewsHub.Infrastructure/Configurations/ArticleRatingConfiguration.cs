using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class ArticleRatingConfiguration : IEntityTypeConfiguration<ArticleRating>
{
    public void Configure(EntityTypeBuilder<ArticleRating> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne<Infrastructure.Identity.ApplicationUser>()
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // one rating per user per article
        builder.HasIndex(r => new { r.ArticleId, r.UserId }).IsUnique();
    }
}