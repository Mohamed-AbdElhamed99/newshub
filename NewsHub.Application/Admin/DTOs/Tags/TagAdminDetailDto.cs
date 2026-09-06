namespace NewsHub.Application.Admin.DTOs.Tags;

public class TagAdminDetailDto
{
    public int Id { get; set; }
    public List<TagTranslationDto> Translations { get; set; } = new();
}