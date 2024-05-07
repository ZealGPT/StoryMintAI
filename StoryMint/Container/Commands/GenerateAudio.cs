using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToAudio;
using StoryMint.Container.Domain;
using StoryMint.Data;

namespace StoryMint.Container.Commands;

public record GenerateAudio(StoryCreated Story, IdentityUser User, string Voice = "alloy", string Format = "mp3", float Speed = 1.0f) : INotification;

public class GenerateAudioHandler(ILogger<GenerateAudioHandler> logger, ApplicationDbContext DbContext, Kernel kernel) : INotificationHandler<GenerateAudio>
{
    private readonly Kernel _kernel = kernel;

    public async Task Handle(GenerateAudio notification, CancellationToken cancellationToken)
    {
        try
        {
            var textToAudioService = _kernel.GetRequiredService<ITextToAudioService>();
            OpenAITextToAudioExecutionSettings executionSettings = new()
            {
                Voice = notification.Voice,
                ResponseFormat = notification.Format,
                Speed = notification.Speed,
            };

            var mediaId = Guid.NewGuid();
            var mediaVersion = DateTime.UtcNow.Ticks;
            var mediaPath = $"wwwroot/media_audio/audio_{mediaVersion}__{notification.Story.Id}__{mediaId}.{notification.Format}";
            var media = new Media()
            {
                Id = mediaId,
                Voice = notification.Voice,
                Path = mediaPath,
                StoryId = notification.Story.Id,
                CreatedBy = notification.User,
                Type = MediaType.Audio,
                Version = mediaVersion,
                Created = DateTime.UtcNow,
            };
            var audioContent = await textToAudioService.GetAudioContentAsync(notification.Story.Content, executionSettings, cancellationToken: cancellationToken);
            if (audioContent != null)
            {
                await File.WriteAllBytesAsync(mediaPath, audioContent.Data?.ToArray()!, cancellationToken: cancellationToken);
            }

            media.Path = mediaPath.Replace("wwwroot", string.Empty);
            await DbContext.AddAsync(media, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Audio generation failed.");
        }
    }
}
