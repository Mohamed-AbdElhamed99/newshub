using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Models.Article;

public class RatingFormViewModel
{
    public int ArticleId { get; set; }

    [Range(1, 5, ErrorMessage = "Pick a rating from 1 to 5.")]
    public int Value { get; set; }
}