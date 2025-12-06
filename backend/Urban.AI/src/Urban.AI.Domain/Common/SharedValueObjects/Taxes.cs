namespace Urban.AI.Domain.Common.SharedValueObjects;

public sealed record Taxes
{
    public decimal Value { get; }
    
    private Taxes(decimal value)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "The Taxes value must be between 0 and 100.");
        }
        
        Value = value;
    }
    
    public static Taxes FromValue(decimal value) => new Taxes(value);
}