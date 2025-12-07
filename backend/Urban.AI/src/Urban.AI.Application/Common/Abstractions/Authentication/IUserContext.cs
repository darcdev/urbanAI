namespace Urban.AI.Application.Common.Abstractions.Authentication;

public interface IUserContext
{
    string Email { get; }

    string IdentityId { get; }

    List<string> Roles { get; }
};