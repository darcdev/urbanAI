namespace Urban.AI.Application.Auth.WhoAmI;

using Urban.AI.Application.Common.Abstractions.CQRS;

public record WhoAmIUserQuery() : IQuery<Dtos.User>;