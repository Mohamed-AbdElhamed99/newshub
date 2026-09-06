namespace NewsHub.Domain.Enums;

public enum CommentStatus
{
    Pending = 0,   // Newly submitted, awaiting moderation
    Approved = 1,  // Visible to readers
    Rejected = 2,  // Explicitly denied by moderators
    Spam = 3,      // Flagged as spam content
    Deleted = 4    // Soft-deleted, hidden but retained for audit
}