using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryMint.Data;
using System.Security.Claims;

namespace StoryMint.Pages.Stories;

public class IndexModel(ApplicationDbContext dbContext) : PageModel
{
    public IReadOnlyList<StoryMint.Container.Domain.Story>? Stories { get; set; }

    public void OnGet()
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        Stories = dbContext.Stories.Where(q => q.CreatedBy.Id == userId).ToList();
    }
}
