namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record PersonalInfoRequest(
    DateOnly BirthDate,
    string Gender,
    string DocumentType,
    string DocumentNumber);
