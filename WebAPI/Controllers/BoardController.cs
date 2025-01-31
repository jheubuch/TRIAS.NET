using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardController
{
    private readonly IBoardService _departureService;

    public BoardController(IBoardService departureService)
    {
        _departureService = departureService;
    }

    [HttpGet("Departures/{locationRef}")]
    public async Task<List<BoardItem>> GetDepartures(
        [FromRoute] string locationRef,
        [FromQuery] DateTime? when,
        [FromQuery] int? count,
        [FromQuery] List<TransportType> transportTypes,
        CancellationToken cancellationToken,
        [FromQuery] bool includeUpcomingStops = false,
        [FromQuery] bool includeRealTimeData = true)
    {
        return await _departureService.GetDepartures(
            locationRef, 
            when ?? DateTime.Now, 
            count, 
            transportTypes, 
            includeUpcomingStops, 
            includeRealTimeData, 
            cancellationToken);
    }

    [HttpGet("Arrivals/{locationRef}")]
    public async Task<List<BoardItem>> GetArrivals(
        [FromRoute] string locationRef,
        [FromQuery] DateTime? when,
        [FromQuery] int? count,
        [FromQuery] List<TransportType> transportTypes,
        CancellationToken cancellationToken,
        [FromQuery] bool includePreviousStops = false,
        [FromQuery] bool includeRealTimeData = true)
    {
        return await _departureService.GetArrivals(
            locationRef,
            when ?? DateTime.Now,
            count,
            transportTypes,
            includePreviousStops,
            includeRealTimeData,
            cancellationToken);
    }
}
