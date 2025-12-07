namespace Urban.AI.Domain.Leaders.Events;

using Urban.AI.Domain.Common.Abstractions;

public sealed record LeaderCreatedDomainEvent(Guid LeaderId) : IDomainEvent;
