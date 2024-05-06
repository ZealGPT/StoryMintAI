using Microsoft.SemanticKernel;

namespace StoryMint.Container;

public class PromptFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        var functionName = context.Function.Name;
        await next(context);

        context.RenderedPrompt += " NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY";
    }
}
