namespace Urban.AI.Application.Organizations.CreateOrganization;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Organizations.Dtos;
#endregion

public sealed record CreateOrganizationCommand(CreateOrganizationRequest Request) : ICommand<Guid>;
