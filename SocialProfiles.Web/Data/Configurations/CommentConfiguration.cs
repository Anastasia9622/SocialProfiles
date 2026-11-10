using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialProfiles.Core.Domain.Profiles;
using SocialProfiles.Core.Domain.Social;

namespace SocialProfiles.Web.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.AuthorId).IsRequired();
        builder.Property(c => c.Text).IsRequired().HasMaxLength(Comment.MaxLength);
        builder.HasIndex(c => c.ProfileId);

        builder.HasOne<Profile>()
               .WithMany()
               .HasForeignKey(c => c.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(c => c.AuthorId)
               .OnDelete(DeleteBehavior.Restrict);

        // Відповідь посилається на батьківський коментар цієї ж таблиці.
        builder.HasOne<Comment>()

               .WithMany()
               .HasForeignKey(c => c.ParentCommentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}