using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.DTOs.Tags;

public class TagFilter : PaginationParams
{
    public string? Name { get; set; }
}