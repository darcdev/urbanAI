namespace Urban.AI.Domain.Users.Events;

using Urban.AI.Domain.Common.Abstractions;

public record UserRegisterDomainEvent(Guid IdUser) : IDomainEvent;