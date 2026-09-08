namespace OddsEngine.Api.Domain;

public static class Value
{
    /// <summary>
    /// Expected value per unit staked. Positive means the bet is
    /// +EV at your assessed probability.
    /// EV = p * (decimal - 1) - (1 - p)
    /// </summary>
    public static decimal ExpectedValuePerUnit(Odds offered, decimal trueProbability)
    {
        if (trueProbability <= 0m || trueProbability >= 1m)
            throw new ArgumentOutOfRangeException(nameof(trueProbability));
        return trueProbability * (offered.Decimal - 1m) - (1m - trueProbability);
    }

    /// <summary>The edge: your probability minus the (vig-inflated) implied one.</summary>
    public static decimal Edge(Odds offered, decimal trueProbability) =>
        trueProbability - offered.ImpliedProbability;
}