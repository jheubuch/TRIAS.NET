using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;
using TRIAS.NET.Models;
using DateTime = System.DateTime;

namespace TRIAS.NET.WebAPI.Services;

public interface IBoardService
{
    public Task<List<BoardItem>> GetDepartures(
        string locationRef, 
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includeUpcomingStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken);

    public Task<List<BoardItem>> GetArrivals(
        string locationRef,
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includePreviousStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken);
}

public class BoardService : TriasHttpService<StopEventRequestStructure, StopEventResponseStructure>, IBoardService
{
    public BoardService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task<List<BoardItem>> GetDepartures(
        string locationRef, 
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includeUpcomingStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken)
    {
        return await RequestData(
            StopEventTypeEnumeration.departure,
            locationRef,
            when,
            count,
            transportTypes,
            includeUpcomingStops,
            includeRealTimeData,
            cancellationToken);
    }

    public async Task<List<BoardItem>> GetArrivals(
        string locationRef,
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includePreviousStops,
        bool includeRealTimeData,
        CancellationToken cancellationToken)
    {
        return await RequestData(
            StopEventTypeEnumeration.arrival,
            locationRef,
            when,
            count,
            transportTypes,
            includePreviousStops,
            includeRealTimeData,
            cancellationToken);
    }

    private async Task<List<BoardItem>> RequestData(
        StopEventTypeEnumeration stopEventType,
        string locationRef,
        DateTime when,
        int? count,
        List<TransportType> transportTypes,
        bool includeOtherStops,
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
                .WithPolicy(stopEventType)
                .IncludeRealtimeData(includeRealTimeData)
        };
        if (stopEventType == StopEventTypeEnumeration.departure)
        {
            request.Params.IncludeUpcomingStops(includeOtherStops);
        } else if (stopEventType == StopEventTypeEnumeration.arrival)
        {
            request.Params.IncludePreviousStops(includeOtherStops);
        }

        var response = await Request(request, cancellationToken);

        return response.StopEventResult.Select(r => r.ToBoardItem(stopEventType)).ToList();
    }
}
