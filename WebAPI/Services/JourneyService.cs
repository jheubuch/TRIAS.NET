using TRIAS.NET.Models.Models;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.WebAPI.Services;

public interface IJourneyService
{
    public Task<Journey> GetJourney(string journeyRef, string operatingDayRef, CancellationToken cancellationToken);
}

public class JourneyService : TriasHttpService<TripInfoRequestStructure, TripInfoResponseStructure>, IJourneyService
{
    public JourneyService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task<Journey> GetJourney(string journeyRef, string operatingDayRef, CancellationToken cancellationToken)
    {
        var request = new TripInfoRequestStructure
        {
            Items = new List<object> { 
                new JourneyRefStructure { Value = journeyRef },
                new OperatingDayRefStructure { Value = operatingDayRef }
            }
        };
        var response = await Request(request, cancellationToken);
        return response.TripInfoResult.ToJourney();
    }
}
