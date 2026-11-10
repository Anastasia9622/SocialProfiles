using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Web.Data.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        // Складений ключ: пара (хто, на кого) не може повторитися, тож дублікати підписок неможливі на рівні бази.
        builder.HasKey(f => new { f.FollowerId, f.FollowedId });

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(f => f.FollowerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(f => f.FollowedId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.FollowedId);
    }
}