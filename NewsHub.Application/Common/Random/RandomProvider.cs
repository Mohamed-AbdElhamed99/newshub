namespace NewsHub.Application.Common.Random;

public class RandomProvider : IRandomProvider
{
    public int Next(int maxExclusive) => System.Random.Shared.Next(maxExclusive);
}