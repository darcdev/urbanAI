namespace Urban.AI.Domain.Common.SharedValueObjects;

public sealed record Currency
{
    internal static readonly Currency None = new Currency(string.Empty);
    public static readonly Currency Usd = new Currency("USD");
    public static readonly Currency Eur = new Currency("EUR");
    public static readonly Currency Cop = new Currency("COP");
    
    private Currency(string code) => Code = code;
    
    public string Code { get; init; }
    
    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ??
               throw new ApplicationException("The currency code is invalid");
    }
    
    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd,
        Eur,
        Cop
    };
};