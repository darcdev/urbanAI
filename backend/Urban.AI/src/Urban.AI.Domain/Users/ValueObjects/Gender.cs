namespace Urban.AI.Domain.Users.ValueObjects;

public sealed record Gender(string Value)
{
    private static readonly Gender Male = new Gender("male");
    private static readonly Gender Female = new Gender("female");
    private static readonly Gender Other = new Gender("other");
    
    public static Gender Create(string value)
    {
        var lowerValue = value.ToLower();
        return All.FirstOrDefault(c => c.Value == lowerValue) ??
               throw new ApplicationException("The gender value is invalid");
    }
    
    public static readonly IReadOnlyCollection<Gender> All = new[]
    {
        Male,
        Female,
        Other
    };
}