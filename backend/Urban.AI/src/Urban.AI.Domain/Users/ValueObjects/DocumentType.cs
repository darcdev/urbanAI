namespace Urban.AI.Domain.Users.ValueObjects;

public sealed record DocumentType
{
    private static readonly DocumentType CC = new DocumentType("cc");
    private static readonly DocumentType Passport = new DocumentType("passport");
    
    public static DocumentType Create(string value)
    {
        var lowerValue = value.ToLower();
        return All.FirstOrDefault(c => c.Value == lowerValue) ??
               throw new ApplicationException("The type document value is invalid");
    }
    
    public static readonly IReadOnlyCollection<DocumentType> All = new[]
    {
        CC,
        Passport
    };
    
    public string Value { get; init; }

    private DocumentType(string value)
    {
        Value = value;
    }
}