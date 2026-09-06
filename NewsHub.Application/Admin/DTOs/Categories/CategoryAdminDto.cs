namespace NewsHub.Application.Admin.DTOs.Categories;

public class CategoryAdminDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
}