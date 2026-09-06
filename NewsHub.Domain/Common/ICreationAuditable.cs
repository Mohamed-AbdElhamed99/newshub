namespace NewsHub.Domain.Common;

public interface ICreationAuditable
{
    DateTime CreatedAt { get; set; }
}