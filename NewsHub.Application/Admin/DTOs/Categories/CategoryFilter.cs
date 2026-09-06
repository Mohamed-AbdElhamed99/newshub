using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.DTOs.Categories;

public class CategoryFilter : PaginationParams
{
    public string? Name { get; set; }
}