using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoryMint.Container.Domain;

namespace StoryMint.Container.Infra;

public class StoryEntityTypeConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Created)
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .IsRequired();

        builder.HasMany(p => p.Media)
            .WithOne(p => p.Story)
            .HasForeignKey(p => p.StoryId)
            .IsRequired();
    }
}
