namespace NewsHub.Application.Admin.DTOs.Tags;

public class UpdateTagDto
{
    public int Id { get; set; }
    public List<TagTranslationDto> Translations { get; set; } = new();
}