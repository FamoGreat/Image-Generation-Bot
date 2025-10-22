using ImageGenerationBotApi.Services;
using ImageGenerationBotApi.Services.IServices;
using ImageGenerationBotApi.Settings;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAISettings"));

builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
    return new OpenAIClient(settings.ApiKey);
});

builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
