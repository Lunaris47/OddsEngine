namespace OddsEngine.Api.Contracts;

public record MarketRequest(List<int> AmericanOdds);

public record ParlayRequest(decimal Stake, List<int> AmericanLegs);

public record EvRequest(int American, decimal TrueProbability);

public record KellyRequest(decimal Bankroll, int American, decimal TrueProbability, decimal Multiplier = 1m);

public record HedgeRequest(decimal OriginalStake, int OriginalAmerican, int HedgeAmerican);