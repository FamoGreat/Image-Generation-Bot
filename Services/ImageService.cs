using ImageGenerationBotApi.Services.IServices;
using ImageGenerationBotApi.Settings;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Images;

namespace ImageGenerationBotApi.Services;

public class ImageService : IImageService
{
    private readonly ImageClient _imageClient;
    public ImageService(OpenAIClient openAIClient, IOptions<OpenAISettings> openAiSettings)
    {
        _imageClient = openAIClient.GetImageClient(openAiSettings.Value.Model);
    }

    public async Task<Stream> GenerateImageAsync(string prompt)
    {
        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1792xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage image = await _imageClient.GenerateImageAsync(prompt, options);

        BinaryData bytes = image.ImageBytes;

        return bytes.ToStream();
    }

    public async Task<Stream> EditImageAsync(string imageFilePath, string prompt, string? maskFilePath = null)
    {
        ImageEditOptions options = new()
        {
            Size = GeneratedImageSize.W512xH512,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        GeneratedImage edit = await _imageClient.GenerateImageEditAsync(imageFilePath, prompt, maskFilePath, options);
        BinaryData bytes = edit.ImageBytes;
        return bytes.ToStream();
    }

}
