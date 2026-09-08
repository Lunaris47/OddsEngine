namespace OddsEngine.Api.Domain;

public static class Hedge
{
    /// <summary>
    /// You hold a bet (stake at odds). The other side is now offered
    /// at hedgeOdds. Returns the hedge stake that locks in an equal
    /// profit (or equal loss) regardless of outcome.
    /// </summary>
    public static decimal EqualProfitHedgeStake(decimal originalStake,
        Odds originalOdds, Odds hedgeOdds) =>
        originalStake * originalOdds.Decimal / hedgeOdds.Decimal;

    /// <summary>Guaranteed profit after placing the equal-profit hedge.</summary>
    public static decimal LockedProfit(decimal originalStake,
        Odds originalOdds, Odds hedgeOdds)
    {
        var hedgeStake = EqualProfitHedgeStake(originalStake, originalOdds, hedgeOdds);
        return originalStake * originalOdds.Decimal - originalStake - hedgeStake;
    }

    /// <summary>
    /// An arbitrage exists when the best available odds on each side
    /// imply probabilities summing to less than 1 (across books).
    /// </summary>
    public static bool IsArbitrage(params Odds[] bestOddsPerOutcome) =>
        Market.Overround(bestOddsPerOutcome) < 1m;

    /// <summary>Guaranteed return per unit of total outlay, if an arb exists.</summary>
    public static decimal ArbitrageReturn(params Odds[] bestOddsPerOutcome)
    {
        var overround = Market.Overround(bestOddsPerOutcome);
        return overround >= 1m ? 0m : (1m / overround) - 1m;
    }
}