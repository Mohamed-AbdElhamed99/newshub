namespace NewsHub.Application.Admin.DTOs.Categories;

public class UpdateCategoryDto
{
    public int Id { get; set; }
    public string? ImagePath { get; set; }
    public List<CategoryTranslationDto> Translations { get; set; } = new();
}