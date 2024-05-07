using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextToImage;
using StoryMint.Container.Commands;
using StoryMint.Container.Domain;
using StoryMint.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace StoryMint.Container;

public class AiService(ILogger<AiService> logger, IMediator mediator, ApplicationDbContext DbContext, Kernel kernel)
{
    private readonly ApplicationDbContext _dbContext = DbContext;
    private readonly Kernel _kernel = kernel;
    private readonly IChatCompletionService _chatService = kernel.GetRequiredService<IChatCompletionService>();

    public async Task<Result<Guid>> CreateStory(CreateStory request, IdentityUser user)
    {
        var storyId = Guid.NewGuid();

        try
        {
            var systemMessage = """
            You are StoryGPT, expert in generating engaging and informative storyteller by the given theme.
            Below qualities should be considered while generating the story.

            You always respond in JSON only, in below format:

            ```json
             {"title": "<Title of the story>", "imageDescription": "<Short description of the story to generate an image>", "content": "<Content of the story, which crafted as per user needs>"}
            ```
            """;

            var userMessage = $"""
                Please generate a story with below qualities:
                Story Genre:  {request.Genre}
                
                Dialogue Generation Tone: {request.Tone}
                
                Target Audience Age Group: {request.AgeGroup}

                """;

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                userMessage += $"""
                    A brief description of the desired story: {request.Description}

                    """;
            }

            userMessage += "Generate the story in given format only.";

            var history = new ChatHistory(systemMessage);
            history.AddUserMessage(userMessage);

            var promptResult = await _chatService.GetChatMessageContentAsync(history, new OpenAIPromptExecutionSettings()
            {
                ChatSystemPrompt = systemMessage,
                Temperature = 0.7,
                MaxTokens = 4096,
                ResponseFormat = "json_object"
            });

            StoryOutput? storyOutput = null;
            try
            {
                var output = JsonNode.Parse(promptResult.ToString()); // JsonSerializer.Deserialize<JsonNode>(promptResult.ToString());
                storyOutput = new StoryOutput(output?["title"]?.ToString() ?? string.Empty,
                    output?["imageDescription"]?.ToString() ?? string.Empty,
                    output?["content"]?.ToString() ?? string.Empty);
            }
            catch (Exception)
            {
            }
            if (storyOutput == null)
            {
                return Result.Error("Failed to generate story, please try again!");
            }

            var story = new Story()
            {
                Genre = request.Genre,
                Tone = request.Tone,
                AgeGroup = request.AgeGroup,
                Content = storyOutput.Content,
                CreatedBy = user,
                Title = storyOutput.Title,
                Description = storyOutput.ImageDescription,
                Created = DateTime.UtcNow
            };
            var storyAdded = await _dbContext.AddAsync(story);
            if (storyAdded?.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                return Result.Error("Failed to add story in database.");
            }

            await _dbContext.SaveChangesAsync();

            var result = new StoryCreated(story.Id, story.Title, story.Description, story.Content);

            await mediator.Publish(new GenerateImage(result, user));
            await mediator.Publish(new GenerateAudio(result, user));
            return Result.Success(story.Id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to generate a story");
            return Result.ErrorWithCorrelationId(storyId.ToString(), ex.Message);
        }
    }
}
