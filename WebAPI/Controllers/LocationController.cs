using Microsoft.AspNetCore.Mvc;
using TRIAS.NET.Models;
using TRIAS.NET.WebAPI.Services;

namespace TRIAS.NET.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController(ILogger<LocationController> logger, ILocationService locationService) : ControllerBase
{
    [HttpGet("search/{query}")]
    public async Task<List<Location>> Search([FromRoute] string query, CancellationToken cancellationToken)
    {
        return await locationService.Search(query, cancellationToken);
    }

    [HttpGet("locate/{latitude}/{longitude}")]
    public async Task<List<Location>> Locate([FromRoute] decimal latitude, [FromRoute] decimal longitude, CancellationToken cancellationToken)
    {
        return await locationService.Locate(new Coordinates { Latitude = latitude, Longitude = longitude }, cancellationToken);
    }
}
