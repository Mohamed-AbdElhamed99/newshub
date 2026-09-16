namespace NewsHub.Application.Site.DTOs.Categories;

public class CategoryListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}