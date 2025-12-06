namespace Urban.AI.Infrastructure.Clock;

#region Usings
using Urban.AI.Application.Common.Abstractions.Clock; 
#endregion

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}