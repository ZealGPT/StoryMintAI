using Microsoft.AspNetCore.Identity;

namespace StoryMint.Container.Domain;

public abstract class AggregateRoot
{
    protected AggregateRoot() => Id = Guid.NewGuid();

    public Guid Id { get; protected init; } = default!;
}

public class Story : AggregateRoot
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string AgeGroup { get; set; } = default!;
    public Constants.Genre Genre { get; set; } = default!;
    public Constants.Tone Tone { get; set; } = default!;

    public IList<Media> Media { get; set; } = [];

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

    public DateTime Created { get; set; }
    public string CreatedById { get; set; } = default!;
    public IdentityUser CreatedBy { get; set; } = default!;

    public DateTime? LastModified { get; set; }
    public IdentityUser? LastModifiedBy { get; set; }
}
