using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models.Models;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JourneyController
{
    private readonly IJourneyService _journeyService;

    public JourneyController(IJourneyService journeyService)
    {
        _journeyService = journeyService;
    }

    [HttpGet("{journeyRef}/{operatingDayRef}")]
    public async Task<Journey> GetJourney([FromRoute] string journeyRef, [FromRoute] string operatingDayRef, CancellationToken cancellationToken)
    {
        return await _journeyService.GetJourney(journeyRef, operatingDayRef, cancellationToken);
    }
}
