namespace NewsHub.Application.Admin.Services;

using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

using NewsHub.Domain.Enums;
public class CommentAdminService
{
    private readonly ICommentRepository _repository;

    public CommentAdminService(ICommentRepository repository)
    {
        this._repository = repository;
    }

    public async Task<PagedResult<CommentAdminDto>> GetCommentsAsync(CommentFilter filter)
    {
        var (comments, totalCount) = await _repository.GetAllAsync(filter);

        var items = comments.Select(MapToDto).ToList();

        return new PagedResult<CommentAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<CommentAdminDto?> ApproveCommentAsync(int id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null) return null;

        comment.Status = CommentStatus.Approved;
        comment.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(comment);
        return MapToDto(comment);
    }

    public async Task<CommentAdminDto?> RejectCommentAsync(int id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null) return null;

        comment.Status = CommentStatus.Rejected;
        comment.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(comment);
        return MapToDto(comment);
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null) return false;

        await _repository.DeleteAsync(comment);
        return true;
    }

    private static CommentAdminDto MapToDto(Comment comment) =>
        new CommentAdminDto
        {
            Id = comment.Id,
            ArticleId = comment.ArticleId,
            UserId = comment.UserId,
            Content = comment.Content,
            Status = comment.Status,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
}