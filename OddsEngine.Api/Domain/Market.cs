namespace OddsEngine.Api.Domain;

public static class Market
{
    /// <summary>Total implied probability across all outcomes. > 1 means vig.</summary>
    public static decimal Overround(params Odds[] outcomes) =>
        outcomes.Sum(o => o.ImpliedProbability);

    /// <summary>The book's margin, e.g. 0.0476 for a standard -110/-110 market.</summary>
    public static decimal Vig(params Odds[] outcomes) =>
        Overround(outcomes) - 1m;

    /// <summary>
    /// Removes the vig by normalizing implied probabilities to sum to 1,
    /// returning the fair (no-vig) odds for each outcome.
    /// </summary>
    public static Odds[] RemoveVig(params Odds[] outcomes)
    {
        var overround = Overround(outcomes);
        return outcomes
            .Select(o => Odds.FromImpliedProbability(o.ImpliedProbability / overround))
            .ToArray();
    }
}