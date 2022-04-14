namespace Common.Models;

public enum ReferenceCurrency
{
    [System.ComponentModel.Description("USD")]
    USD,

    [System.ComponentModel.Description("EUR")]
    EUR,
}

public record Currency
{
    public string Symbol { get; init; } = default!;

    public string Name { get; init; } = default!;

    public decimal Price { get; init; } = default;

    public double? PriceChangeOneDay { get; init; } = default;

    public double? PriceChangeSevenDays { get; init; } = default;

    public double? PriceChangeThirtyDays { get; init; } = default;
}

public class CurrencySet : Dictionary<string, Currency>
{
    public CurrencySet() { }
    public CurrencySet(Dictionary<string, Currency> d) : base (d) { }
}

public class CurrencyDb : Dictionary<ReferenceCurrency, CurrencySet>
{
    public CurrencyDb() { }
    public CurrencyDb(Dictionary<ReferenceCurrency, CurrencySet> d) : base(d) { }
}