using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;
using TRIAS.NET.Models;
using DateTime = System.DateTime;

namespace TRIAS.NET.WebAPI.Services;

public interface IDepartureService
{
    public Task GetDepartures(
        string locationRef, 
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includeUpcomingStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken);
}

public class DepartureService : TriasHttpService<StopEventRequestStructure, StopEventResponseStructure>, IDepartureService
{
    public DepartureService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task GetDepartures(
        string locationRef, 
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includeUpcomingStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken)
    {
        var request = new StopEventRequestStructure
        {
            Location = new LocationContextStructure
            {
                DepArrTime = when,
                Item = new LocationRefStructure
                {
                    Item = new StopPointRefStructure
                    {
                        Value = locationRef
                    },
                    LocationName = new[]
                    {
                        new InternationalTextStructure
                        {
                            Text = ""
                        }
                    }.ToList()
                },
            },
            Params = new StopEventParamStructure()
                .WithFilter(transportTypes)
                .WithPolicy(count)
                .IncludeUpcomingStops(includeUpcomingStops)
                .IncludeRealtimeData(includeRealTimeData)
        };
        var response = await Request(request, cancellationToken);
    }
}
