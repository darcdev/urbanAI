namespace Urban.AI.Domain.Users.ValueObjects;

public sealed class PersonalInfo
{
    #region Constants
    public const int MaxDocumentNumberLength = 50;
    #endregion

    private PersonalInfo() { }

    public PersonalInfo(DateOnly birthDate,
                        Gender gender,
                        DocumentType documentType,
                        string documentNumber)
    {
        BirthDate = birthDate;
        Gender = gender;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
    }

    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; }
}
