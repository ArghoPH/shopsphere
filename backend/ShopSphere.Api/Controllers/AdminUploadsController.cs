using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ShopSphere.Api.Controllers;

[ApiController]
[Route("api/admin/uploads")]
[Authorize(Roles = "Admin,MasterAdmin")]
public class AdminUploadsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public AdminUploadsController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("product-image")]
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Image file is required" });
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { message = "Image size must be under 5MB" });
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest(new { message = "Only JPG, PNG and WEBP images are allowed" });
        }

        var supabaseUrl = _configuration["Supabase:Url"];
        var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];

        if (string.IsNullOrWhiteSpace(supabaseUrl) ||
            string.IsNullOrWhiteSpace(serviceRoleKey))
        {
            return StatusCode(500, new { message = "Supabase storage configuration is missing" });
        }

        supabaseUrl = supabaseUrl.TrimEnd('/');

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var objectPath = $"products/{fileName}";

        var uploadUrl = $"{supabaseUrl}/storage/v1/object/product-images/{objectPath}";

        var client = _httpClientFactory.CreateClient();

        await using var stream = file.OpenReadStream();

        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
        request.Headers.Add("apikey", serviceRoleKey);
        request.Headers.Add("x-upsert", "false");
        request.Content = content;

        var response = await client.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                message = "Failed to upload image to Supabase Storage",
                details = responseText
            });
        }

        var publicUrl = $"{supabaseUrl}/storage/v1/object/public/product-images/{objectPath}";

        return Ok(new
        {
            message = "Image uploaded successfully",
            imageUrl = publicUrl
        });
    }
}