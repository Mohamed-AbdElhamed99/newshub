using NewsHub.Application.Site.DTOs.Articles;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IHomePageService
{
    Task<IEnumerable<CategoryArticlesDto>> GetWhatIsNewAsync(int articlesPerCategory);
    Task<CategoryArticlesDto> GetLifeStyleSectionAsync(int articleCount);
}