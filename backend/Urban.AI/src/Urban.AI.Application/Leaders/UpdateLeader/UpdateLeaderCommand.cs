namespace Urban.AI.Application.Leaders.UpdateLeader;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Leaders.Dtos;
#endregion

public sealed record UpdateLeaderCommand(
    Guid LeaderId,
    UpdateLeaderRequest Request) : ICommand;
