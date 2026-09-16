namespace NewsHub.Application.Common.Random;

public interface IRandomProvider
{
    int Next(int maxExclusive);
}