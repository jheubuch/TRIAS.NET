using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models.Models;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController
{
    private readonly ITripService _tripService;

    public TripController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpPost]
    public async Task<List<Trip>> PlanTrip([FromBody] TripPlanRequest tripPlanRequest)
    {
        return await _tripService.PlanTrip(tripPlanRequest, CancellationToken.None);
    }
}
