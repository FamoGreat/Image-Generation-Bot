using Microsoft.AspNetCore.Mvc;

namespace ImageGenerationBotApi.Models;

public class GenerateImageRequest
{
    public string Prompt { get; set; } = string.Empty;
}

public class EditImageRequest
{
    [FromForm]
    public IFormFile Image { get; set; } = null!;

    [FromForm]
    public string Prompt { get; set; } = string.Empty;

    [FromForm]
    public IFormFile? Mask { get; set; }
}
