using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoryMint.Data;

namespace StoryMint.Pages.Stories;

public class DetailsModel(ApplicationDbContext dbContext) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; } = default!;

    public StoryMint.Container.Domain.Story? Story { get; set; }

    public async Task<IActionResult> OnGet()
    {
        Story = await dbContext.Stories.Include(q => q.Media).FirstOrDefaultAsync(q => q.Id == Id);

        if (Story == null)
            return RedirectToPage("./Index");

        return Page();
    }
}
