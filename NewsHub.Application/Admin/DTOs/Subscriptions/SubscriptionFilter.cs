using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.DTOs.Subscriptions;

public class SubscriptionFilter: PaginationParams
{
    public bool? IsActive { get; set; }
}