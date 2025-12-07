namespace Urban.AI.Domain.Organizations.Events;

using Urban.AI.Domain.Common.Abstractions;

public sealed record OrganizationCreatedDomainEvent(Guid OrganizationId) : IDomainEvent;
