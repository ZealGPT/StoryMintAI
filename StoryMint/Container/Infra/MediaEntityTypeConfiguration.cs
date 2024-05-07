using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoryMint.Container.Domain;

namespace StoryMint.Container.Infra;

public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Created)
          .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .IsRequired();

        builder.HasIndex(x => x.Type).IsUnique(false);
        builder.HasIndex(x => x.Version).IsUnique(false);
    }
}
