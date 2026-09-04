namespace OddsEngine.Api.Domain;

/// <summary>
/// Immutable representation of betting odds. Internally stored as
/// decimal odds; all other formats convert through it.
/// </summary>
public readonly record struct Odds
{
    public decimal Decimal { get; }

    private Odds(decimal decimalOdds)
    {
        if (decimalOdds <= 1m)
            throw new ArgumentOutOfRangeException(nameof(decimalOdds),
                "Decimal odds must be greater than 1.");
        Decimal = decimalOdds;
    }

    public static Odds FromDecimal(decimal decimalOdds) => new(decimalOdds);

    public static Odds FromAmerican(int american)
    {
        if (american == 0 || (american > -100 && american < 100))
            throw new ArgumentOutOfRangeException(nameof(american),
                "American odds must be <= -100 or >= +100.");

        decimal dec = american > 0
            ? 1m + american / 100m
            : 1m + 100m / Math.Abs(american);

        return new Odds(dec);
    }

    public static Odds FromImpliedProbability(decimal probability)
    {
        if (probability <= 0m || probability >= 1m)
            throw new ArgumentOutOfRangeException(nameof(probability),
                "Probability must be strictly between 0 and 1.");
        return new Odds(1m / probability);
    }

    public int ToAmerican()
    {
        return Decimal >= 2m
            ? (int)Math.Round((Decimal - 1m) * 100m)
            : (int)Math.Round(-100m / (Decimal - 1m));
    }

    public decimal ImpliedProbability => 1m / Decimal;
}