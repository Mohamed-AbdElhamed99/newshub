namespace NewsHub.Application.Admin.DTOs.Tags;

public class CreateTagDto
{
    public List<TagTranslationDto> Translations { get; set; } = new();
}