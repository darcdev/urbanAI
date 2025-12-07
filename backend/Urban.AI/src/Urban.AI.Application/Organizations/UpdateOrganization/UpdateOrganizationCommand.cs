namespace Urban.AI.Application.Organizations.UpdateOrganization;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Organizations.Dtos;
#endregion

public sealed record UpdateOrganizationCommand(
    Guid OrganizationId,
    UpdateOrganizationRequest Request) : ICommand;
