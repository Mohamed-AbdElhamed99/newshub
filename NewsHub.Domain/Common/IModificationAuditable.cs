namespace NewsHub.Domain.Common;

public interface IModificationAuditable
{
    DateTime UpdatedAt { get; set; }
}