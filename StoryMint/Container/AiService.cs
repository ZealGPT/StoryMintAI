using Ardalis.Result;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextToImage;

namespace StoryMint.Container;

public class AiService(ILogger<AiService> logger, Kernel kernel)
{
    private readonly ILogger<AiService> _logger = logger;
    private readonly Kernel _kernel = kernel;
    private readonly ITextEmbeddingGenerationService _embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
    private readonly IChatCompletionService _chatService = kernel.GetRequiredService<IChatCompletionService>();
    private readonly ITextToImageService _imageService = kernel.GetRequiredService<ITextToImageService>();

    public async Task<Result<StoryCreated>> CreateStory(CreateStory request)
    {
        var storyId = Guid.NewGuid();

        try
        {
            var prompt = $$$"""
Instructions:
You are StoryGPT, expert in generating engaging and informative storyteller by the given theme.
You can only generate stories. Below qualities should be considered while generating the story.

Story Genre: {{$genre}}

Dialogue Generation Tone: {{$tone}}

Target Audience Age Group: {{$ageGroup}}
""";

            var kernelArguments = new KernelArguments()
            {
                ["genre"] = request.Genre,
                ["tone"] = request.Tone,
                ["ageGroup"] = request.AgeGroup
            };

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                prompt += """

                    A brief description of the desired story: {{$description}}
                    """;

                kernelArguments.Add("description", request.Description);
            }

            var promptResult = await _kernel.InvokePromptAsync(prompt, kernelArguments);

            var imageResult = await _imageService.GenerateImageAsync(@$"Generate a attarctive story image using story theme {request.Genre}", 1024, 1024);

            var result = new StoryCreated(storyId, "Story Title", "Story Description", promptResult.ToString(), imageResult);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to generate a story");
            return Result.ErrorWithCorrelationId(storyId.ToString(), "Server error, please try again!");
        }
    }
}
