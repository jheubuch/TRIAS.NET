using System;
using System.Collections.Generic;
using System.Linq;
using TRIAS.NET.Models.Trias;
using DateTime = System.DateTime;

namespace TRIAS.NET.Models.Models;
public record class Journey
{
    public string JourneyRef { get; set; }
    public string OperatingDayRef { get; set; }
    public string Line { get; set; }
    public List<JourneyStop> Stops { get; set; }
}

public record class JourneyStop
{
    public string LocationRef { get; set; }
    public string Name { get; set; }
    public DateTime? ScheduledArrival { get; set; }
    public DateTime? ScheduledDeparture { get; set; }
    public DateTime? RealArrival { get; set; }
    public DateTime? RealDeparture { get; set; }
    public string ScheduledPlatform { get; set; }
    public string RealPlatform { get; set; }
}

public static class JourneyExtensions
{
    public static JourneyStop ToJourneyStop(this CallAtStopStructure callAtStop)
    {
        var scheduledArrival = callAtStop.ServiceArrival?.TimetabledTime;
        var realArrival = callAtStop.ServiceArrival?.EstimatedTime;
        var hasArrivalRealTime = false;
        if (scheduledArrival != null && realArrival != null)
        {
            hasArrivalRealTime = (scheduledArrival - realArrival) < TimeSpan.FromDays(365 * 2000);
        }
        var scheduledDeparture = callAtStop.ServiceDeparture?.TimetabledTime;
        var realDeparture = callAtStop.ServiceDeparture?.EstimatedTime;
        var hasDepartureRealTime = false;
        if (scheduledDeparture != null && realDeparture != null)
        {
            hasDepartureRealTime = (scheduledDeparture - realDeparture) < TimeSpan.FromDays(365 * 2000);
        }

        return new JourneyStop
        {
            LocationRef = callAtStop.StopPointRef.Value,
            Name = callAtStop.StopPointName.First().Text,
            ScheduledArrival = scheduledArrival,
            ScheduledDeparture = scheduledDeparture,
            RealArrival = hasArrivalRealTime ? realArrival : null,
            RealDeparture = hasDepartureRealTime ? realDeparture : null,
            ScheduledPlatform = callAtStop.PlannedBay.FirstOrDefault()?.Text ?? "",
            RealPlatform = callAtStop.EstimatedBay.FirstOrDefault()?.Text
        };
    }

    public static Journey ToJourney(this TripInfoResultStructure trip)
    {
        var journey = new Journey
        {
            JourneyRef = trip.Service.JourneyRef.Value,
            OperatingDayRef = trip.Service.OperatingDayRef.Value,
            Line = string.Join(", ", trip.Service.ServiceSection.Select(s => s.PublishedLineName.First().Text)),
            Stops = new List<JourneyStop>()
        };

        journey.Stops.AddRange(trip.PreviousCall.Select(ToJourneyStop));
        journey.Stops.AddRange(trip.OnwardCall.Select(ToJourneyStop));

        return journey;
    }
}
