using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartureController
{
    private readonly IDepartureService _departureService;

    public DepartureController(IDepartureService departureService)
    {
        _departureService = departureService;
    }

    [HttpGet("{locationRef}")]
    public async Task GetDepartures(
        [FromRoute] string locationRef,
        [FromQuery] DateTime? when,
        [FromQuery] int? count,
        [FromQuery] List<TransportType> transportTypes,
        CancellationToken cancellationToken,
        [FromQuery] bool includeUpcomingStops = false,
        [FromQuery] bool includeRealTimeData = true)
    {
        await _departureService.GetDepartures(
            locationRef, 
            when ?? DateTime.Now, 
            count, 
            transportTypes, 
            includeUpcomingStops, 
            includeRealTimeData, 
            cancellationToken);
    }
}
