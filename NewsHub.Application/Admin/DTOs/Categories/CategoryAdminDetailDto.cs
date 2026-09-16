namespace NewsHub.Application.Admin.DTOs.Categories;

public class CategoryAdminDetailDto
{
    public int Id { get; set; }
    public string? ImagePath { get; set; }
    public List<CategoryTranslationDto> Translations { get; set; } = new();
}