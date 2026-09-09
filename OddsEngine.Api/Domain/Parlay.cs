namespace OddsEngine.Api.Domain;

public static class Parlay
{
    public static Odds CombinedOdds(params Odds[] legs)
    {
        if (legs.Length < 2)
            throw new ArgumentException("A parlay needs at least 2 legs.");
        return Odds.FromDecimal(legs.Aggregate(1m, (acc, leg) => acc * leg.Decimal));
    }

    public static decimal Payout(decimal stake, params Odds[] legs) =>
        stake * CombinedOdds(legs).Decimal;

    public static decimal Profit(decimal stake, params Odds[] legs) =>
        Payout(stake, legs) - stake;

    /// <summary>
    /// True combined probability of the parlay hitting, using
    /// no-vig probabilities if you pass fair odds — or the book's
    /// inflated view if you pass listed odds. Comparing the two is
    /// exactly how you see why books love parlays.
    /// </summary>
    public static decimal HitProbability(params Odds[] legs) =>
        legs.Aggregate(1m, (acc, leg) => acc * leg.ImpliedProbability);
}