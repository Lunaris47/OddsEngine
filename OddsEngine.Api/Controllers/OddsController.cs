using Microsoft.AspNetCore.Mvc;
using OddsEngine.Api.Domain;

namespace OddsEngine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OddsController : ControllerBase
{
    // GET /api/odds/convert?american=-150
    [HttpGet("convert")]
    public IActionResult Convert([FromQuery] int american)
    {
        var odds = Odds.FromAmerican(american);
        return Ok(new
        {
            american,
            @decimal = Math.Round(odds.Decimal, 4),
            impliedProbability = Math.Round(odds.ImpliedProbability, 4)
        });
    }
}