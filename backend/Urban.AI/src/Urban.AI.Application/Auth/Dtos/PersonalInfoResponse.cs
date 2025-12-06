namespace Urban.AI.Application.Auth.Dtos;

public record PersonalInfoResponse(
    DateOnly BirthDate,
    string Gender,
    string DocumentType,
    string DocumentNumber);