using Microsoft.AspNetCore.Identity;

namespace StoryMint.Container.Domain;

public enum MediaType
{
    Audio,
    Image,
    Video
}

public class Media
{
    public Guid Id { get; set; }
    public MediaType Type { get; set; }
    public string Path { get; set; } = string.Empty;

    public long Version { get; set; }

    public string Voice { get; set; } = string.Empty;
    public string Dimension { get; set; } = string.Empty;

    public Guid StoryId { get; set; }
    public Story Story { get; set; } = default!;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

    public DateTime Created { get; set; }
    public string CreatedById { get; set; } = default!;
    public IdentityUser CreatedBy { get; set; } = default!;


    public DateTime? LastModified { get; set; }
    public IdentityUser? LastModifiedBy { get; set; }
}