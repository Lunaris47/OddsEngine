using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Contracts;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/hedge")]
public class HedgeController : ControllerBase
{
    // POST /api/hedge/equal-profit
    // Body: { "originalStake": 100, "originalAmerican": 1000, "hedgeAmerican": -200 }
    [HttpPost("equal-profit")]
    public IActionResult EqualProfit([FromBody] HedgeRequest request)
    {
        if (request.OriginalStake <= 0m)
            return BadRequest("Original stake must be greater than zero.");

        try
        {
            var original = Odds.FromAmerican(request.OriginalAmerican);
            var hedge = Odds.FromAmerican(request.HedgeAmerican);

            var hedgeStake = Hedge.EqualProfitHedgeStake(request.OriginalStake, original, hedge);
            var locked = Hedge.LockedProfit(request.OriginalStake, original, hedge);

            return Ok(new
            {
                hedgeStake = Math.Round(hedgeStake, 2),
                lockedProfit = Math.Round(locked, 2),
                isProfitable = locked > 0m,
                totalOutlay = Math.Round(request.OriginalStake + hedgeStake, 2)
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST /api/hedge/arbitrage
    // Body: { "americanOdds": [105, 105] }  <- best available odds per outcome, across books
    [HttpPost("arbitrage")]
    public IActionResult Arbitrage([FromBody] MarketRequest request)
    {
        if (request.AmericanOdds is null || request.AmericanOdds.Count < 2)
            return BadRequest("At least two outcomes are required.");

        try
        {
            var odds = request.AmericanOdds.Select(Odds.FromAmerican).ToArray();
            var overround = Market.Overround(odds);
            var isArb = Hedge.IsArbitrage(odds);

            return Ok(new
            {
                isArbitrage = isArb,
                overround = Math.Round(overround, 4),
                guaranteedReturnPercent = Math.Round(Hedge.ArbitrageReturn(odds) * 100m, 2),
                stakeProportions = odds.Select(o => new
                {
                    american = o.ToAmerican(),
                    proportionOfBudget = Math.Round(o.ImpliedProbability / overround, 4)
                })
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}