using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Contracts;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/value")]
public class ValueController : ControllerBase
{
    // POST /api/value/ev
    // Body: { "american": 110, "trueProbability": 0.5 }
    [HttpPost("ev")]
    public IActionResult ExpectedValue([FromBody] EvRequest request)
    {
        try
        {
            var offered = Odds.FromAmerican(request.American);
            var ev = Value.ExpectedValuePerUnit(offered, request.TrueProbability);

            return Ok(new
            {
                offeredAmerican = request.American,
                offeredImpliedProbability = Math.Round(offered.ImpliedProbability, 4),
                trueProbability = request.TrueProbability,
                evPerUnit = Math.Round(ev, 4),
                edge = Math.Round(Value.Edge(offered, request.TrueProbability), 4),
                isPositiveEV = ev > 0m
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}