using System.ComponentModel.DataAnnotations;
using NewsHub.Application.Site.DTOs.Articles;

namespace NewsHub.Web.Models.Article;

public class CommentFormViewModel
{
    public int ArticleId { get; set; }

    [Required(ErrorMessage = "Write something before posting.")]
    [StringLength(2000, MinimumLength = 2)]
    public string Content { get; set; } = string.Empty;
}