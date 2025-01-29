using System;
using System.Text.Json.Serialization;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<TransportType>))]
public enum TransportType
{
    All,
    Unknown,
    Air,
    Bus,
    TrolleyBus,
    Tram,
    Coach,
    Rail,
    IntercityRail,
    UrbanRail,
    Metro,
    Water,
    Cableway,
    Funicular,
    Taxi
}

public static class TransportTypeExtensions
{
    public static PtModesEnumeration ToPtModesEnumeration(this TransportType transportType)
    {
        return transportType switch
        {
            TransportType.All => PtModesEnumeration.all,
            TransportType.Unknown => PtModesEnumeration.unknown,
            TransportType.Air => PtModesEnumeration.air,
            TransportType.Bus => PtModesEnumeration.bus,
            TransportType.TrolleyBus => PtModesEnumeration.trolleyBus,
            TransportType.Tram => PtModesEnumeration.tram,
            TransportType.Coach => PtModesEnumeration.coach,
            TransportType.Rail => PtModesEnumeration.rail,
            TransportType.IntercityRail => PtModesEnumeration.intercityRail,
            TransportType.UrbanRail => PtModesEnumeration.urbanRail,
            TransportType.Metro => PtModesEnumeration.metro,
            TransportType.Water => PtModesEnumeration.water,
            TransportType.Cableway => PtModesEnumeration.cableway,
            TransportType.Funicular => PtModesEnumeration.funicular,
            TransportType.Taxi => PtModesEnumeration.taxi,
            _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
        };
    }

    public static TransportType ToTransportType(this PtModesEnumeration ptModesEnumeration)
    {
        return ptModesEnumeration switch
        {
            PtModesEnumeration.all => TransportType.All,
            PtModesEnumeration.unknown => TransportType.Unknown,
            PtModesEnumeration.air => TransportType.Air,
            PtModesEnumeration.bus => TransportType.Bus,
            PtModesEnumeration.trolleyBus => TransportType.TrolleyBus,
            PtModesEnumeration.tram => TransportType.Tram,
            PtModesEnumeration.coach => TransportType.Coach,
            PtModesEnumeration.rail => TransportType.Rail,
            PtModesEnumeration.intercityRail => TransportType.IntercityRail,
            PtModesEnumeration.urbanRail => TransportType.UrbanRail,
            PtModesEnumeration.metro => TransportType.Metro,
            PtModesEnumeration.water => TransportType.Water,
            PtModesEnumeration.cableway => TransportType.Cableway,
            PtModesEnumeration.funicular => TransportType.Funicular,
            PtModesEnumeration.taxi => TransportType.Taxi,
            _ => throw new ArgumentOutOfRangeException(nameof(ptModesEnumeration), ptModesEnumeration, null)
        };
    }
}
