using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Contracts;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/parlay")]
public class ParlayController : ControllerBase
{
    /// <summary>
    /// Prices a multi-leg parlay: combined odds, total payout, profit, and true hit probability.
    /// </summary>

    // POST /api/parlay/price
    // Body: { "stake": 10, "americanLegs": [-110, -110, -110] }
    [HttpPost("price")]
    public IActionResult Price([FromBody] ParlayRequest request)
    {
        if (request.Stake <= 0m)
            return BadRequest("Stake must be greater than zero.");
        if (request.AmericanLegs is null || request.AmericanLegs.Count < 2)
            return BadRequest("A parlay requires at least two legs.");

        try
        {
            var legs = request.AmericanLegs.Select(Odds.FromAmerican).ToArray();
            var combined = Parlay.CombinedOdds(legs);

            return Ok(new
            {
                legCount = legs.Length,
                combinedAmerican = combined.ToAmerican(),
                combinedDecimal = Math.Round(combined.Decimal, 4),
                payout = Math.Round(Parlay.Payout(request.Stake, legs), 2),
                profit = Math.Round(Parlay.Profit(request.Stake, legs), 2),
                hitProbability = Math.Round(Parlay.HitProbability(legs), 4)
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        } 
    }
}