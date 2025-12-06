namespace Urban.AI.Application.Common.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }

    string IdentityId { get; }
};