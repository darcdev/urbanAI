namespace Urban.AI.Application.Common.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}