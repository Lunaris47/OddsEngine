using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Contracts;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/staking")]
public class StakingController : ControllerBase
{
    // POST /api/staking/kelly
    // Body: { "bankroll": 1000, "american": 100, "trueProbability": 0.55 }
    // Optional: "multiplier": 0.5 for half Kelly, (defaultS to 1 = full Kelly)
    [HttpPost("kelly")]
    public IActionResult Kelly([FromBody] KellyRequest request)
    {
        if (request.Bankroll <= 0m)
            return BadRequest("Bankroll must be greater than zero.");
        if (request.Multiplier <= 0m || request.Multiplier > 1m)
            return BadRequest("Multiplier must be between 0 (exclusive) and 1 (inclusive).");

        try
        {
            var offered = Odds.FromAmerican(request.American);
            var fraction = Staking.KellyFraction(offered, request.TrueProbability, request.Multiplier);
            var stake = Staking.KellyStake(request.Bankroll, offered, request.TrueProbability, request.Multiplier);

            return Ok(new
            {
                kellyFraction = Math.Round(fraction, 4),
                recommendedStake = Math.Round(stake, 2),
                multiplierUsed = request.Multiplier,
                isBetRecommended = fraction > 0m
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}        