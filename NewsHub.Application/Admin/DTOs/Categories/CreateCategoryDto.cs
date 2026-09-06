namespace NewsHub.Application.Admin.DTOs.Categories;

public class CreateCategoryDto
{
    public string? ImagePath { get; set; }
    public List<CategoryTranslationDto> Translations { get; set; } = new();
}