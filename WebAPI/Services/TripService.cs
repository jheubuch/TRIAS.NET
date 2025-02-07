using TRIAS.NET.Models;
using TRIAS.NET.Models.Models;
using TRIAS.NET.Models.Trias;
using TRIAS.NET.WebAPI.Helper;
using TRIAS.NET.Models.Models.Enums;
using DateTime = System.DateTime;

namespace TRIAS.NET.WebAPI.Services;

public interface ITripService
{
    public Task<List<Trip>> PlanTrip(TripPlanRequest tripPlanRequest, CancellationToken cancellationToken);
}

public class TripService : TriasHttpService<TripRequestStructure, TripResponseStructure>, ITripService
{
    public TripService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
    {
    }

    public async Task<List<Trip>> PlanTrip(TripPlanRequest tripPlanRequest, CancellationToken cancellationToken)
    {
        if (!(tripPlanRequest.DepartingFrom == null ^ tripPlanRequest.ArrivingAt == null))
        {
            throw new TriasException(400, "Either departure or arrival has to be specified.");
        }

        var tripRequest = new TripRequestStructure
        {
            Origin = new List<LocationContextStructure>
            { 
                new LocationContextStructure 
                { 
                    Item = new LocationRefStructure { Item = tripPlanRequest.Origin.ToLocationRefStructure() }
                } 
            },
            Destination = new List<LocationContextStructure> 
            { 
                new LocationContextStructure 
                { 
                    Item = new LocationRefStructure { Item = tripPlanRequest.Destination.ToLocationRefStructure() }
                } 
            }
        };

        if (tripPlanRequest.DepartingFrom != null)
        {
            tripRequest.Origin.First().DepArrTime = (DateTime) tripPlanRequest.DepartingFrom;
        } 
        else if (tripPlanRequest.ArrivingAt != null)
        {
            tripRequest.Destination.First().DepArrTime = (DateTime) tripPlanRequest.ArrivingAt;
        }

        tripRequest.Via = tripPlanRequest.ViaStops?.Select(via =>
        {
            return new ViaStructure
            {
                ViaPoint = new LocationRefStructure { Item = via.ToLocationRefStructure() }
            };
        }).ToList();
        tripRequest.NotVia = tripPlanRequest.NotViaStops?.Select(notVia =>
        {
            return new NotViaStructure
            {
                Item = notVia.ToLocationRefStructure()
            };
        }).ToList();
        tripRequest.NoChangeAt = tripPlanRequest.NoChangeAtStops?.Select(noChangeAt =>
        {
            return new NoChangeAtStructure
            {
                Item = noChangeAt.ToLocationRefStructure()
            };
        }).ToList();

        if (tripPlanRequest.Params != null)
        {
            tripRequest.Params = new TripParamStructure
            {
                IgnoreRealtimeData = tripPlanRequest.Params.IgnoreRealtime,
                InterchangeLimit = tripPlanRequest.Params.InterchangeLimit?.ToString() ?? null,
                PtModeFilter = tripPlanRequest.Params.TransportFilter == null ? null : new PtModeFilterStructure
                {
                    PtMode = tripPlanRequest.Params.TransportFilter?.Select(t => t.ToPtModesEnumeration()).ToList(),
                    Exclude = false
                },
                LevelEntrance = tripPlanRequest.Params.AccessibilityParams.LevelEntrance,
                NoSingleStep = tripPlanRequest.Params.AccessibilityParams.NoSingleStep,
                NoStairs = tripPlanRequest.Params.AccessibilityParams.NoStairs,
                NoEscalator = tripPlanRequest.Params.AccessibilityParams.NoEscalators,
                NoElevator = tripPlanRequest.Params.AccessibilityParams.NoElevators,
                NoRamp = tripPlanRequest.Params.AccessibilityParams.NoRamp,
                BikeTransport = tripPlanRequest.Params.BikeTransport,
                IncludeIntermediateStops = tripPlanRequest.IncludeIntermediateStops,
                AlgorithmType = tripPlanRequest.Algorithm.ToAlgorithmTypeEnumeration()
            };
        }

        var response = await Request(tripRequest, cancellationToken);
        if (response.ErrorMessage != null && response.ErrorMessage.Count > 0)
        {
            throw new TriasException(400, string.Join(", ", response.ErrorMessage.Select(e => e.Text.First().Text)));
        }
        return response.TripResult.Select(res => res.ToTrip()).ToList();
    }
}
