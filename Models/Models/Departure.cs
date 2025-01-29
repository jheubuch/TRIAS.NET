using System.Collections.Generic;
using System.Linq;
using TRIAS.NET.Models.Models.Enums;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models;

public record class Departure
{
}

public static class DepartureExtensions
{
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

    public static StopEventParamStructure IncludeUpcomingStops(this StopEventParamStructure structure, bool includeUpcomingStops = true)
    {
        structure.IncludeOnwardCalls = includeUpcomingStops;
        return structure;
    }

    public static StopEventParamStructure IncludeRealtimeData(this StopEventParamStructure structure, bool includeRealtimeData = true)
    {
        structure.IncludeRealtimeData = includeRealtimeData;
        return structure;
    }
}
