namespace OddsEngine.Api.Domain;

public static class Staking
{
    /// <summary>
    /// Optimal fraction of bankroll to stake. Returns 0 for -EV bets
    /// (Kelly never bets a negative edge). Multiplier scales it:
    /// 0.5 = half-Kelly, the common risk-reduced choice.
    /// </summary>
    public static decimal KellyFraction(Odds offered, decimal trueProbability,
        decimal multiplier = 1m)
    {
        var b = offered.Decimal - 1m;
        var p = trueProbability;
        var q = 1m - p;
        var fraction = (b * p - q) / b;
        return Math.Max(0m, fraction * multiplier);
    }

    public static decimal KellyStake(decimal bankroll, Odds offered,
        decimal trueProbability, decimal multiplier = 1m) =>
        bankroll * KellyFraction(offered, trueProbability, multiplier);
}