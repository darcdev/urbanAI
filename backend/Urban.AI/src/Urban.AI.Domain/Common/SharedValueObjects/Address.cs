namespace Urban.AI.Domain.Common.SharedValueObjects;

public sealed record Address(
    string Country,
    string State,
    string City,
    string ZipCode,
    string Street);