using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Article;
using NewsHub.Web.Models.Shared;

namespace NewsHub.Web.Controllers;

public class ArticleController : Controller
{
    private const int PageSize = 12;

    private readonly IArticleService _articleService;
    private readonly IArticleInteractionService _interactionService;
    private readonly ITagService _tagService;
    private readonly IUserService _userService;

    public ArticleController(IArticleService articleService, IArticleInteractionService interactionService , ITagService tagService , IUserService userService)
    {
        _articleService = articleService;
        _interactionService = interactionService;
        _tagService = tagService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var vm = await BuildListViewModelAsync(new ArticleListFilter { Page = page, PageSize = PageSize },
            "Latest News", searchTerm: null);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string q, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction(nameof(Index));

        var vm = await BuildListViewModelAsync(
            new ArticleListFilter { Page = page, PageSize = PageSize, SearchTerm = q },
            $"Search results for \"{q}\"", q);

        return View("Index", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var article = await _articleService.GetArticleDetailBySlugAsync(slug);
        if (article is null)
            return NotFound();

        await _articleService.RegisterViewAsync(article.Id);

        var comments = await _interactionService.GetApprovedCommentsAsync(article.Id);

        var vm = new ArticleDetailsViewModel
        {
            Article = article,
            Comments = comments,
            CommentForm = new CommentFormViewModel { ArticleId = article.Id },
            RatingForm = new RatingFormViewModel { ArticleId = article.Id },
            CanInteract = User.Identity?.IsAuthenticated ?? false,
            PopularNews = await _articleService.GetPopularAsync(4),
            TrendingTags = await _tagService.GetTrendingTagsAsync(8)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CommentFormViewModel form, string slug)
    {
        var userId = _userService.GetCurrentUserAsync();
        if (userId is null)
        {
            TempData["InteractionError"] = "Sign in to leave a comment.";
            return RedirectToAction(nameof(Details), new { slug });
        }

        if (!ModelState.IsValid)
        {
            TempData["InteractionError"] = "Comment couldn't be posted — check what you wrote and try again.";
            return RedirectToAction(nameof(Details), new { slug });
        }

        await _interactionService.AddCommentAsync(new CommentDto
        {
            ArticleId = form.ArticleId,
            UserId = userId.Result,
            Content = form.Content
        });

        TempData["InteractionSuccess"] = "Comment posted — it'll show once it's been reviewed.";
        return RedirectToAction(nameof(Details), new { slug });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rate(RatingFormViewModel form, string slug)
    {
        var userId = _userService.GetCurrentUserAsync();
        if (userId is null)
        {
            TempData["InteractionError"] = "Sign in to rate this article.";
            return RedirectToAction(nameof(Details), new { slug });
        }

        if (!ModelState.IsValid)
        {
            TempData["InteractionError"] = "That's not a valid rating.";
            return RedirectToAction(nameof(Details), new { slug });
        }

        await _interactionService.RateArticleAsync(new RatingDto
        {
            ArticleId = form.ArticleId,
            UserId = userId.Result,
            Value = form.Value
        });

        TempData["InteractionSuccess"] = "Thanks for rating!";
        return RedirectToAction(nameof(Details), new { slug });
    }

    private async Task<ArticleListViewModel> BuildListViewModelAsync(ArticleListFilter filter, string heading, string? searchTerm)
    {
        var result = await _articleService.GetPagedAsync(filter);
        var totalPages = result.PageSize == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);

        return new ArticleListViewModel
        {
            Result = result,
            Heading = heading,
            SearchTerm = searchTerm,
            Pagination = new PaginationViewModel
            {
                CurrentPage = result.Page,
                TotalPages = totalPages,
                Controller = "Article",
                Action = searchTerm is null ? "Index" : "Search",
                RouteValues = searchTerm is null ? null : new { q = searchTerm }
            }
        };
    }
    
}