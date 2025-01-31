using System;
using System.Text.Json.Serialization;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<LocationType>))]
public enum LocationType
{
    StopPoint,
    StopPlace,
    Locality,
    PointOfInterest,
    Address
}

public static class LocationTypeExtensions
{
    public static LocationTypeEnumeration FromLocationType(this LocationType locationType)
    {
        return locationType switch
        {
            LocationType.StopPoint => LocationTypeEnumeration.stop,
            LocationType.StopPlace => LocationTypeEnumeration.stop,
            LocationType.Locality => LocationTypeEnumeration.locality,
            LocationType.PointOfInterest => LocationTypeEnumeration.poi,
            LocationType.Address => LocationTypeEnumeration.address,
            _ => throw new ArgumentOutOfRangeException(nameof(locationType), locationType, null)
        };
    }
}
