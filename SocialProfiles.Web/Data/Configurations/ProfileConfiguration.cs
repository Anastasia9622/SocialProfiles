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

        // Один користувач = один профіль.
        builder.Property(p => p.UserId).IsRequired();
        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasOne<ApplicationUser>()
               .WithOne()
               .HasForeignKey<Profile>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // NOCASE: порівняння без урахування регістру, "Anna" і "anna" вважаються однаковими.
        builder.Property(p => p.Username).IsRequired().HasMaxLength(20).UseCollation("NOCASE");
        builder.HasIndex(p => p.Username).IsUnique();

        builder.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.About).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.AvatarPath).HasMaxLength(300);
        builder.Property(p => p.BannerPath).HasMaxLength(300);

        // Тема не має власної таблиці: її поля зберігаються у стовпцях таблиці Profiles.
        builder.OwnsOne(p => p.Theme, theme =>
        {
            theme.Property(t => t.BackgroundColor).HasColumnName("BackgroundColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.CardColor).HasColumnName("CardColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.TextColor).HasColumnName("TextColor").HasMaxLength(7).IsRequired();
            theme.Property(t => t.AccentColor).HasColumnName("AccentColor").HasMaxLength(7).IsRequired();
        });
        builder.Navigation(p => p.Theme).IsRequired();

        // Блоки: EF працює напряму з приватним списком _blocks, а не з копією з властивості Blocks.
        builder.HasMany(p => p.Blocks)
               .WithOne()
               .HasForeignKey("ProfileId")
               .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Blocks)
               .HasField("_blocks")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}