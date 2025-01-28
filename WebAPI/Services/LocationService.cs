using TRIAS.NET.Models;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.WebAPI.Services;
public interface ILocationService
{
    public Task<List<NET.Models.Location>> Search(string query, CancellationToken cancellationToken);
    public Task<List<NET.Models.Location>> Locate(Coordinates coordinates, CancellationToken cancellationToken);
}

public class LocationService : TriasHttpService<LocationInformationRequestStructure, LocationInformationResponseStructure>, ILocationService
{
    public LocationService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task<List<NET.Models.Location>> Search(string query, CancellationToken cancellationToken)
    {
        var locationRequest = new LocationInformationRequestStructure
        {
            Item = new InitialLocationInputStructure
            {
                LocationName = query
            }
        };
        var response = await Request(locationRequest, cancellationToken);
        return response.LocationResult.Select(l => l.ToLocation()).ToList();
    }

    public async Task<List<NET.Models.Location>> Locate(Coordinates coordinates, CancellationToken cancellationToken)
    {
        var locationRequest = new LocationInformationRequestStructure
        {
            Item = new InitialLocationInputStructure
            {
                GeoPosition = new GeoPositionStructure
                {
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude
                }
            }
        };
        var response = await Request(locationRequest, cancellationToken);
        return response.LocationResult.Select(l => l.ToLocation()).ToList();
    }
}

