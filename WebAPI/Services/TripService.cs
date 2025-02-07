using TRIAS.NET.Models;
using TRIAS.NET.Models.Models;
using TRIAS.NET.Models.Trias;
using DateTime = System.DateTime;

namespace TRIAS.NET.WebAPI.Services;

public interface ITripService
{
    public Task PlanTrip(TripPlanRequest tripPlanRequest, CancellationToken cancellationToken);
}

public class TripService : TriasHttpService<TripRequestStructure, TripResponseStructure>, ITripService
{
    public TripService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task PlanTrip(TripPlanRequest tripPlanRequest, CancellationToken cancellationToken)
    {
        var tripRequest = new TripRequestStructure
        {
            Origin = new List<LocationContextStructure> { new LocationContextStructure { Item = new LocationRefStructure { Item = tripPlanRequest.Origin.ToLocationRefStructure() }, DepArrTime = DateTime.Now } },
            Destination = new List<LocationContextStructure> { new LocationContextStructure { Item = new LocationRefStructure { Item = tripPlanRequest.Destination.ToLocationRefStructure() } } }
        };

        var response = await Request(tripRequest, cancellationToken);
    }
}
