using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryMint.Container;

namespace StoryMint.Pages.Stories;

[Authorize]
public class CreateModel(AiService aiService) : PageModel
{
    private readonly AiService _aiService = aiService;

    [BindProperty]
    public CreateStory? StoryRequest { get; set; }
    public StoryCreated? StoryCreated { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (StoryRequest == null)
        {
            ModelState.AddModelError("", "Please fill form and submit again!");
            return Page();
        }

        var story = await _aiService.CreateStory(StoryRequest);

        if (story.IsSuccess)
        {            
            StoryCreated = story;
            return Page();
        }

        ModelState.AddModelError("", story.Errors.FirstOrDefault() ?? "Failed to create Story, please try again!");
        return Page();
    }
}
