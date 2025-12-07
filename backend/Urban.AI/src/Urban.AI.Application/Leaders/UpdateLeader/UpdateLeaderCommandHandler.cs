namespace Urban.AI.Application.Leaders.UpdateLeader;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Leaders;
using Urban.AI.Domain.Users;
#endregion

internal sealed class UpdateLeaderCommandHandler : ICommandHandler<UpdateLeaderCommand>
{
    private readonly ILeaderRepository _leaderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeaderCommandHandler(
        ILeaderRepository leaderRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _leaderRepository = leaderRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateLeaderCommand request, CancellationToken cancellationToken)
    {
        var leader = await _leaderRepository.GetByIdWithDetailsAsync(request.LeaderId, cancellationToken);
        if (leader is null)
        {
            return Result.Failure(LeaderErrors.LeaderNotFound);
        }

        var user = await _userRepository.GetByIdAsync(leader.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(LeaderErrors.UserNotFound);
        }

        user.UpdateBasicInfo(
            user.Email,
            request.Request.FirstName,
            request.Request.LastName);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
