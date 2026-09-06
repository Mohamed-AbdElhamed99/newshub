using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.DTOs.ContactMessages;

public class ContactMessageFilter : PaginationParams
{
    public bool? IsRead { get; set; }
}