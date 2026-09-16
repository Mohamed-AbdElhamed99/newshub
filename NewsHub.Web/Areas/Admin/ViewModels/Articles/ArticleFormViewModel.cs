using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsHub.Domain.Enums;

namespace NewsHub.Web.Areas.Admin.ViewModels.Articles;

public class ArticleFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public bool IsTrending { get; set; }

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Featured Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Tags")]
    public List<int> SelectedTagIds { get; set; } = new();

    public ArticleTranslationViewModel English { get; set; } = new() { LanguageCode = "en" };
    public ArticleTranslationViewModel Arabic { get; set; } = new() { LanguageCode = "ar" };

    // Populated by the controller for dropdowns — not bound from the form
    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> TagOptions { get; set; } = new();
}