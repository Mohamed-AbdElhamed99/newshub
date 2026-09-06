namespace NewsHub.Application.Admin.DTOs.Categories;

public class CategoryTranslationDto
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
}