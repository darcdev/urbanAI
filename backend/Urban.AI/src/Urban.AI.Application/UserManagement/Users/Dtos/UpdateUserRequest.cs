namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email);
