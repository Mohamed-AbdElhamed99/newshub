namespace NewsHub.Application.Common.Random;

public class RandomProvider
{
    public int Next(int maxExclusive) => System.Random.Shared.Next(maxExclusive);
}