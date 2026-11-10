using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialProfiles.Core.Domain.Blocks;

namespace SocialProfiles.Web.Data.Configurations;

public class ProfileBlockConfiguration : IEntityTypeConfiguration<ProfileBlock>
{
    public void Configure(EntityTypeBuilder<ProfileBlock> builder)
    {
        builder.ToTable("ProfileBlocks");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        // Усі типи блоків живуть в одній таблиці, а стовпець BlockType каже, який це нащадок.
        builder.HasDiscriminator<string>("BlockType")
               .HasValue<AboutBlock>("About")
               .HasValue<CommentsBlock>("Comments")
               .HasValue<FollowersBlock>("Followers");
    }
}