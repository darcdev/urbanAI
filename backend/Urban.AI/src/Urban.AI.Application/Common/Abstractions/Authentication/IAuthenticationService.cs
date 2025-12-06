namespace Urban.AI.Application.Common.Abstractions.Authentication;

#region Usings
using Urban.AI.Domain.Users; 
#endregion

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);
}
