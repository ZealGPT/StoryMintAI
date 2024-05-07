using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using StoryMint.Container.Domain;
using StoryMint.Data;

namespace StoryMint.Container.Commands;

/// <summary>
/// Image Dimensions: 1024x1024, 1792x1024, or 1024x1792
/// </summary>
/// <param name="Story">Created Story</param>
/// <param name="User">User</param>
/// <param name="Width">1024 or 1792</param>
/// <param name="Height">1024 or 1792</param>
public record GenerateImage(StoryCreated Story, IdentityUser User, int Width = 1792, int Height = 1024) : INotification;

public class GenerateImageHandler(ILogger<GenerateImageHandler> logger, ApplicationDbContext DbContext, Kernel kernel) : INotificationHandler<GenerateImage>
{
    private readonly Kernel _kernel = kernel;

    public async Task Handle(GenerateImage notification, CancellationToken cancellationToken)
    {
        try
        {
            var prompt = $"""
                You are a StoryGPT artist, you will be given a image decription of a story and you will generate an attractive and cute image out it.

                Image Description:
                {notification.Story.Description}
                """;
            var textToImageService = _kernel.GetRequiredService<ITextToImageService>();
            var imageUrl = await textToImageService.GenerateImageAsync(prompt, notification.Width, notification.Height, cancellationToken: cancellationToken);

            if (imageUrl != null)
            {
                var mediaId = Guid.NewGuid();
                var mediaVersion = DateTime.UtcNow.Ticks;
                var mediaPath = $"wwwroot/media_images/image_{mediaVersion}__{notification.Story.Id}__{mediaId}.png";
                var media = new Media()
                {
                    Id = mediaId,
                    Dimension = $"{notification.Width}x{notification.Height}",
                    Path = mediaPath,
                    StoryId = notification.Story.Id,
                    CreatedBy = notification.User,
                    Type = MediaType.Image,
                    Version = mediaVersion,
                    Created = DateTime.UtcNow
                };

                try
                {
                    using var client = new HttpClient();
                    var response = await client.GetAsync(imageUrl, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    await File.WriteAllBytesAsync(mediaPath, imageBytes, cancellationToken);

                    media.Path = mediaPath.Replace("wwwroot", string.Empty);
                    await DbContext.AddAsync(media, cancellationToken);
                    await DbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Error downloading image");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Image generation failed");
        }
    }
}
