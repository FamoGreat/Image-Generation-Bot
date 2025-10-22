using ImageGenerationBotApi.Models;
using ImageGenerationBotApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ImageGenerationBotApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IImageService imageService, ILogger<ImageController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateImageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            var imageStream = await _imageService.GenerateImageAsync(request.Prompt);
            return File(imageStream, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image");
            return StatusCode(500, new { error = "Failed to generate image" });
        }
    }

    [HttpPost("edit")]
    public async Task<IActionResult> Edit([FromForm] EditImageRequest request)
    {
        string? imagePath = null;
        string? maskPath = null;

        try
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest(new { error = "Image file is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { error = "Prompt is required" });
            }

            // Save the uploaded image to a temp file
            imagePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(imagePath))
            {
                await request.Image.CopyToAsync(stream);
            }

            // Save the mask file if provided
            if (request.Mask != null && request.Mask.Length > 0)
            {
                maskPath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(maskPath))
                {
                    await request.Mask.CopyToAsync(stream);
                }
            }

            var imageStream = await _imageService.EditImageAsync(imagePath, request.Prompt, maskPath);
            return File(imageStream, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing image");
            return StatusCode(500, new { error = "Failed to edit image" });
        }
        finally
        {
            // Clean up temp files
            if (imagePath != null && System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            if (maskPath != null && System.IO.File.Exists(maskPath))
            {
                System.IO.File.Delete(maskPath);
            }
        }
    }
}