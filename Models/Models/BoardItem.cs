using System;
using System.Collections.Generic;
using System.Linq;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;
using DateTime = System.DateTime;

namespace TRIAS.NET.Models;

public record class BoardItem
{
    public string JourneyRef { get; set; }
    public string OperatingDayRef { get; set; }
    public string Line { get; set; }
    public string Location { get; set; }
    public DateTime ScheduledTime { get; set; }
    public DateTime? RealTime { get; set; }
    public string ScheduledPlatform { get; set; }
    public string RealPlatform { get; set; }
    public List<ViaStop> ViaStops { get; set; }
}

public record class ViaStop
{
    public string Name { get; set; }
    public string LocationRef { get; set; }
    public bool DemandStop { get; set; }
}

public static class BoardExtensions
{
    public static BoardItem ToBoardItem(this StopEventResultStructure result, StopEventTypeEnumeration stopEventType)
    {
        var currentServiceSection = result.StopEvent.Service.ServiceSection.First();
        if (result.StopEvent.Service.ServiceSection.Count > 1)
        {
            var currentStopSeqNumber = int.Parse(result.StopEvent.ThisCall.CallAtStop.StopSeqNumber);
            currentServiceSection = result.StopEvent.Service.ServiceSection.Where(s => int.Parse(s.FromStopSeqNumber) >= currentStopSeqNumber && int.Parse(s.ToStopSeqNumber) <= currentStopSeqNumber).First();
        }
        var scheduledTime = 
            stopEventType == StopEventTypeEnumeration.departure ? 
            result.StopEvent.ThisCall.CallAtStop.ServiceDeparture.TimetabledTime : 
            result.StopEvent.ThisCall.CallAtStop.ServiceArrival.TimetabledTime;

        var realTime = 
            stopEventType == StopEventTypeEnumeration.departure ? 
            result.StopEvent.ThisCall.CallAtStop.ServiceDeparture.EstimatedTime :
            result.StopEvent.ThisCall.CallAtStop.ServiceArrival.EstimatedTime;

        var location =
            stopEventType == StopEventTypeEnumeration.departure ?
            result.StopEvent.Service.DestinationText.First().Text :
            result.StopEvent.Service.OriginText.First().Text;

        var hasRealTime = (scheduledTime - realTime) < TimeSpan.FromDays(365);

        var viaStops = new List<ViaStop>();
        viaStops.AddRange(
            stopEventType == StopEventTypeEnumeration.departure ? 
            result.StopEvent.OnwardCall.Select(s => s.CallAtStop.ToViaStop()) : 
            result.StopEvent.PreviousCall.Select(s => s.CallAtStop.ToViaStop()));

        return new BoardItem
        {
            JourneyRef = result.StopEvent.Service.JourneyRef.Value,
            OperatingDayRef = result.StopEvent.Service.OperatingDayRef.Value,
            Line = currentServiceSection.PublishedLineName.First().Text,
            Location = location,
            ScheduledTime = scheduledTime,
            RealTime = hasRealTime ? realTime : null,
            ScheduledPlatform = result.StopEvent.ThisCall.CallAtStop.PlannedBay.FirstOrDefault()?.Text ?? "",
            RealPlatform = result.StopEvent.ThisCall.CallAtStop.EstimatedBay?.FirstOrDefault()?.Text,
            ViaStops = viaStops
        };
    }

    public static ViaStop ToViaStop(this CallAtStopStructure call)
    {
        return new ViaStop
        {
            LocationRef = call.StopPointRef.Value,
            Name = call.StopPointName.First().Text,
            DemandStop = call.DemandStop
        };
    }

    public static StopEventParamStructure WithFilter(this StopEventParamStructure structure, List<TransportType> transportTypeFilter)
    {
        structure.PtModeFilter = new PtModeFilterStructure
        {
            Exclude = false,
            PtMode = transportTypeFilter.Select(t => t.ToPtModesEnumeration()).ToList()
        };
        return structure;
    }

    public static StopEventParamStructure WithPolicy(this StopEventParamStructure structure, int? count)
    {
        if (count != null)
        {
            structure.NumberOfResults = count.ToString();
        }
        return structure;
    }

    public static StopEventParamStructure WithPolicy(this StopEventParamStructure structure, StopEventTypeEnumeration stopEventType)
    {
        structure.StopEventType = stopEventType;
        return structure;
    }

    public static StopEventParamStructure IncludeUpcomingStops(this StopEventParamStructure structure, bool includeUpcomingStops = true)
    {
        structure.IncludeOnwardCalls = includeUpcomingStops;
        return structure;
    }

    public static StopEventParamStructure IncludePreviousStops(this StopEventParamStructure structure, bool includePreviousStops = true)
    {
        structure.IncludePreviousCalls = includePreviousStops;
        return structure;
    }

    public static StopEventParamStructure IncludeRealtimeData(this StopEventParamStructure structure, bool includeRealtimeData = true)
    {
        structure.IncludeRealtimeData = includeRealtimeData;
        return structure;
    }
}
