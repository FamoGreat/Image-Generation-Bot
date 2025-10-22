namespace ImageGenerationBotApi.Services.IServices;

public interface IImageService
{
    Task<Stream> GenerateImageAsync(string prompt);
    Task<Stream> EditImageAsync(string imageFilePath, string prompt, string? maskFilePath = null);

}
