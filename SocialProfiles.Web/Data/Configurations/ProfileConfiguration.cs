using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Web.Data.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.UserId).IsRequired();
        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasOne<ApplicationUser>()
               .WithOne()
               .HasForeignKey<Profile>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Username).IsRequired().HasMaxLength(20).UseCollation("NOCASE");
        builder.HasIndex(p => p.Username).IsUnique();

        builder.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.About).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.AvatarPath).HasMaxLength(300);
        builder.Property(p => p.BannerPath).HasMaxLength(300);
        builder.Property(p => p.BackgroundImagePath).HasMaxLength(300);

        builder.OwnsOne(p => p.Theme, theme =>
        {
            theme.Property(t => t.BackgroundColor).HasColumnName("BackgroundColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.CardColor).HasColumnName("CardColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.TextColor).HasColumnName("TextColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.AccentColor).HasColumnName("AccentColor").HasMaxLength(7).IsRequired();
        });
        builder.Navigation(p => p.Theme).IsRequired();

        builder.HasMany(p => p.Blocks)
               .WithOne()
               .HasForeignKey("ProfileId")
               .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Blocks)
               .HasField("_blocks")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}