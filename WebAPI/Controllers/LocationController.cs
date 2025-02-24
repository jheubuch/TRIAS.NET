using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController(ILogger<LocationController> logger, ILocationService locationService) : ControllerBase
{
    [HttpGet("Search/{query}")]
    public async Task<List<Location>> Search([FromRoute] string query, [FromQuery] List<LocationType> filterTypes, CancellationToken cancellationToken)
    {
        return await locationService.Search(query, filterTypes, cancellationToken);
    }

    [HttpGet("Locate/{latitude}/{longitude}")]
    public async Task<List<Location>> Locate([FromRoute] decimal latitude, [FromRoute] decimal longitude, [FromQuery] List<LocationType> filterTypes, CancellationToken cancellationToken)
    {
        return await locationService.Locate(new Coordinates { Latitude = latitude, Longitude = longitude }, filterTypes, cancellationToken);
    }
}
