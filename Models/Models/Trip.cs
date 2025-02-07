using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;
using DateTime = System.DateTime;

namespace TRIAS.NET.Models.Models;

public record class TripPlanRequest
{
    public LocationReference Origin { get; set; }
    public LocationReference Destination { get; set; }
    public DateTime? DepartingFrom { get; set; } = DateTime.Now;
    public DateTime? ArrivingAt { get; set; } = null;
    public List<LocationReference> ViaStops { get; set; } = null;
    public List<LocationReference> NotViaStops { get; set; } = null;
    public List<LocationReference> NoChangeAtStops { get; set; } = null;
    public bool IncludeIntermediateStops { get; set; } = true;
    public TripPlanParams Params { get; set; } = new TripPlanParams();
    public TripPlanAlgorithm Algorithm { get; set; } = TripPlanAlgorithm.Fastest;
}

[JsonConverter(typeof(JsonStringEnumConverter<TripPlanAlgorithm>))]
public enum TripPlanAlgorithm
{
    Fastest,
    MinChanges,
    LeastWalking,
    LeastCost
}

public record class TripPlanParams
{
    public List<TransportType> TransportFilter { get; set; } = null;
    public bool IgnoreRealtime { get; set; } = false;
    public int? InterchangeLimit { get; set; } = null;
    public bool BikeTransport { get; set; } = false;
    public AccessibilityParams AccessibilityParams { get; set; } = new AccessibilityParams();
}

public record class AccessibilityParams
{
    public bool NoSingleStep { get; set; } = false;
    public bool NoStairs { get; set; } = false;
    public bool NoEscalators { get; set; } = false;
    public bool NoElevators { get; set; } = false;
    public bool NoRamp { get; set; } = false;
    public bool LevelEntrance { get; set; } = false;
}

public record class Trip
{
    public string TripId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int InterchangeCount { get; set; }
    public List<TripLeg> Legs { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<TripLegType>))]
public enum TripLegType
{
    Timed,
    Interchange,
    Continuous
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "LegType")]
[JsonDerivedType(typeof(TimedLeg), typeDiscriminator: "Timed")]
[JsonDerivedType(typeof(InterchangeLeg), typeDiscriminator: "Interchange")]
[JsonDerivedType(typeof(ContinuousLeg), typeDiscriminator: "Continuous")]
public abstract record class TripLeg
{
    public string LegId { get; set; }
    public TripLegType LegType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

}

public record class TimedLeg : TripLeg
{
    public LegStop Board { get; set; }
    public LegStop Alight { get; set; }
    public List<LegStop> Intermediates { get; set; }
    public string Line { get; set; }
    public string JourneyRef { get; set; }
    public TransportType TransportType { get; set; }
}

public record class InterchangeLeg : TripLeg
{
    public Location Start { get; set; }
    public Location End { get; set; }
}

public record class ContinuousLeg : TripLeg
{
    public Location Start { get; set; }
    public Location End { get; set; }
}

public record class LegStop : ViaStop
{

}

public static class TripExtensions
{
    public static Trip ToTrip(this TripResultStructure tripResponse)
    {
        return new Trip
        {
            TripId = tripResponse.ResultId,
            StartTime = tripResponse.Trip.StartTime,
            EndTime = tripResponse.Trip.EndTime,
            InterchangeCount = int.Parse(tripResponse.Trip.Interchanges),
            Legs = tripResponse.Trip.TripLeg.Select(ToTripLeg).ToList()
        };
    }

    public static TripLeg ToTripLeg(this TripLegStructure tripLegStructure)
    {
        var leg = tripLegStructure.Item;
        if (leg is TimedLegStructure timedLeg)
        {
            return new TimedLeg
            {
                LegId = tripLegStructure.LegId,
                LegType = TripLegType.Timed,
                StartTime = timedLeg.LegBoard.ServiceDeparture.TimetabledTime,
                EndTime = timedLeg.LegAlight.ServiceArrival.TimetabledTime,
                Board = timedLeg.LegBoard.ToLegStop(),
                Alight = timedLeg.LegAlight.ToLegStop(),
                Intermediates = timedLeg.LegIntermediates.Select(ToLegStop).ToList(),
                Line = timedLeg.Service.ServiceSection.First().PublishedLineName.First().Text,
                JourneyRef = timedLeg.Service.JourneyRef.Value,
                TransportType = timedLeg.Service.ServiceSection.First().Mode.PtMode.ToTransportType(),
            };
        } else if (leg is InterchangeLegStructure interchangeLeg)
        {
            return new InterchangeLeg
            {
                LegId = tripLegStructure.LegId,
                LegType = TripLegType.Interchange,
                StartTime = interchangeLeg.TimeWindowStart,
                EndTime = interchangeLeg.TimeWindowEnd,
                Start = interchangeLeg.LegStart.ToLocation(),
                End = interchangeLeg.LegEnd.ToLocation()
            };
        }
        else if (leg is ContinuousLegStructure continuousLeg)
        {
            return new ContinuousLeg
            {
                LegId = tripLegStructure.LegId,
                LegType = TripLegType.Continuous,
                StartTime = continuousLeg.TimeWindowStart,
                EndTime = continuousLeg.TimeWindowEnd,
                Start = continuousLeg.LegStart.ToLocation(),
                End = continuousLeg.LegEnd.ToLocation()
            };
        }
        else
        {
            throw new ArgumentOutOfRangeException("Invalid leg type");
        }
    }

    public static LegStop ToLegStop(this LegBoardStructure legBoard)
    {
        return new LegStop
        {
            Name = legBoard.StopPointName.First().Text,
            LocationRef = legBoard.StopPointRef.Value,
            DemandStop = legBoard.DemandStop
        };
    }

    public static LegStop ToLegStop(this LegAlightStructure legAlight)
    {
        return new LegStop
        {
            Name = legAlight.StopPointName.First().Text,
            LocationRef = legAlight.StopPointRef.Value,
            DemandStop = legAlight.DemandStop
        };
    }

    public static LegStop ToLegStop(this LegIntermediateStructure intermediateStop)
    {
        return new LegStop
        {
            Name = intermediateStop.StopPointName.First().Text,
            LocationRef = intermediateStop.StopPointRef.Value,
            DemandStop = intermediateStop.DemandStop
        };
    }

    public static AlgorithmTypeEnumeration ToAlgorithmTypeEnumeration(this TripPlanAlgorithm algorithm)
    {
        return algorithm switch
        {
            TripPlanAlgorithm.Fastest => AlgorithmTypeEnumeration.fastest,
            TripPlanAlgorithm.MinChanges => AlgorithmTypeEnumeration.minChanges,
            TripPlanAlgorithm.LeastWalking => AlgorithmTypeEnumeration.leastWalking,
            TripPlanAlgorithm.LeastCost => AlgorithmTypeEnumeration.leastCost,
            _ => throw new ArgumentOutOfRangeException("Invalid algorithm type")
        };
    }
}
