﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models;
public record class Location
{
    public LocationType LocationType { get; set; }
    public Coordinates Coordinates { get; set; }
    public string Name { get; set; }
    public string LocationName { get; set; }
    public string LocationRef { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<LocationType>))]
public enum LocationType
{
    StopPoint,
    StopPlace,
    Locality,
    PointOfInterest,
    Address
}

public record class Coordinates
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public static class LocationExtensions
{
    public static Location ToLocation(this LocationResultStructure locationResult)
    {
        var item = locationResult.Location.Item;
        var location = new Location
        {
            Coordinates = new Coordinates
            {
                Latitude = locationResult.Location.GeoPosition.Latitude,
                Longitude = locationResult.Location.GeoPosition.Longitude
            }
        };
        location.LocationName = locationResult.Location.LocationName.First().Text;
        if (item is StopPointStructure stopPoint)
        {
            location.LocationType = LocationType.StopPoint;
            location.Name = stopPoint.StopPointName.First().Text;
            location.LocationRef = stopPoint.StopPointRef.Value;
        }
        else if (item is StopPlaceStructure stopPlace)
        {
            location.LocationType = LocationType.StopPlace;
            location.Name = stopPlace.StopPlaceName.First().Text;
            location.LocationRef = stopPlace.StopPlaceRef.Value;
        }
        else if (item is LocalityStructure locality)
        {
            location.LocationType = LocationType.Locality;
            location.Name = locality.LocalityName.First().Text;
            location.LocationRef = locality.LocalityCode;
        }
        else if (item is PointOfInterestStructure pointOfInterest)
        {
            location.LocationType = LocationType.PointOfInterest;
            location.Name = pointOfInterest.PointOfInterestName.First().Text;
            location.LocationRef = pointOfInterest.PointOfInterestCode;
        }
        else if (item is AddressStructure address)
        {
            location.LocationType = LocationType.Address;
            location.Name = address.AddressName.First().Text;
            location.LocationRef = address.AddressCode;
        }
        return location;
    }

    public static LocationTypeEnumeration FromLocationType(this LocationType locationType)
    {
        return locationType switch
        {
            LocationType.StopPoint => LocationTypeEnumeration.stop,
            LocationType.StopPlace => LocationTypeEnumeration.stop,
            LocationType.Locality => LocationTypeEnumeration.locality,
            LocationType.PointOfInterest => LocationTypeEnumeration.poi,
            LocationType.Address => LocationTypeEnumeration.address,
            _ => LocationTypeEnumeration.stop
        };
    }

    public static LocationInformationRequestStructure WithFilters(this LocationInformationRequestStructure structure, List<LocationType> locationTypeFilter)
    {
        if (structure.Restrictions == null)
        {
            structure.Restrictions = new LocationParamStructure();
        }
        structure.Restrictions.Type = locationTypeFilter.Select(FromLocationType).ToList();
        return structure;
    }
}