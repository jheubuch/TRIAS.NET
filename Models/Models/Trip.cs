using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models.Models;

public record class TripPlanRequest
{
    public SearchLocation Origin { get; set; }
    public SearchLocation Destination { get; set; }
}

public record class Trip
{
    public string TripId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int InterchangeCount { get; set; }
    public List<TripLeg> Legs { get; set; }
}

public enum TripLegType
{
    Timed,
    Interchange,
    Continuous
}

public abstract record class TripLeg
{
    public string LegId { get; set; }
    public TripLegType LegType { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

}

public record class TimedLeg : TripLeg
{
    public ViaStop Board { get; set; }
    public ViaStop Alight { get; set; }
    public List<ViaStop> Intermediates { get; set; }
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

public static class TripExtensions
{
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
                Board = timedLeg.,
                Alight = timedLeg.Alight.ToViaStop(),
                Intermediates = timedLeg.Intermediate.Select(i => i.ToViaStop()).ToList(),
                Line = timedLeg.Line.PublishedLineName.First().Text,
                JourneyRef = timedLeg.Service.JourneyRef.Value,
                TransportType = timedLeg.Service.ServiceSection.First().Mode.PtMode.ToTransportType(),
            }
        }
    }
}
