using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Contracts;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/market")]
public class MarketController : ControllerBase
{
    // POST /api/market/novig
    // Body: { "americanOdds": [-110, -110] }
    [HttpPost("novig")]
    public IActionResult RemoveVig([FromBody] MarketRequest request)
    {
        if (request.AmericanOdds is null || request.AmericanOdds.Count < 2)
            return BadRequest("At least two outcomes are required.");

        try
        {
            var odds = request.AmericanOdds
                .Select(Odds.FromAmerican)
                .ToArray();

            var fair = Market.RemoveVig(odds);

            return Ok(new
            {
                overround = Math.Round(Market.Overround(odds), 4),
                vigPercent = Math.Round(Market.Vig(odds) * 100m, 2),
                outcomes = odds.Zip(fair, (listed, fairOdds) => new
                {
                    listedAmerican = listed.ToAmerican(),
                    listedImpliedProbability = Math.Round(listed.ImpliedProbability, 4),
                    fairAmerican = fairOdds.ToAmerican(),
                    fairProbability = Math.Round(fairOdds.ImpliedProbability, 4)
                })
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}