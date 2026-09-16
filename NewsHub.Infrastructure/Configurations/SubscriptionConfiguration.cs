using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(s => s.Email).IsUnique();
        builder.HasIndex(s => s.UnsubscribeToken).IsUnique();
    }
}