using System;
using System.Collections.Generic;
using System.Linq;
using TRIAS.NET.Models.Models.Enums;
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

public record class Coordinates
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public record class LocationReference
{
    public LocationType LocationType { get; set; }
    public string LocationRef { get; set; }
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

    public static Location ToLocation(this LocationRefStructure locationRefStructure)
    {
        var location = new Location();
        var item = locationRefStructure.Item;
        location.LocationName = locationRefStructure.LocationName.First().Text;
        if (item is StopPointRefStructure stopPoint)
        {
            location.LocationType = LocationType.StopPoint;
            location.Name = stopPoint.Value;
        }
        else if (item is StopPlaceRefStructure stopPlace)
        {
            location.LocationType = LocationType.StopPlace;
            location.LocationRef = stopPlace.Value;
        }
        else if (item is LocalityRefStructure locality)
        {
            location.LocationType = LocationType.Locality;
            location.LocationRef = locality.Value;
        }
        else if (item is PointOfInterestRefStructure pointOfInterest)
        {
            location.LocationType = LocationType.PointOfInterest;
            location.LocationRef = pointOfInterest.Value;
        }
        else if (item is AddressRefStructure address)
        {
            location.LocationType = LocationType.Address;
            location.LocationRef = address.Value;
        } else
        {
            throw new ArgumentOutOfRangeException("Invalid location ref");
        }
        return location;
    }

    public static object ToLocationRefStructure(this LocationReference searchLocation)
    {
        return searchLocation.LocationType switch
        {
            LocationType.StopPoint => new StopPointRefStructure { Value = searchLocation.LocationRef },
            LocationType.StopPlace => new StopPlaceRefStructure { Value = searchLocation.LocationRef },
            LocationType.Locality => new LocalityRefStructure { Value = searchLocation.LocationRef },
            LocationType.PointOfInterest => new PointOfInterestRefStructure { Value = searchLocation.LocationRef },
            LocationType.Address => new AddressRefStructure { Value = searchLocation.LocationRef },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static LocationInformationRequestStructure WithFilter(this LocationInformationRequestStructure structure, List<LocationType> locationTypeFilter)
    {
        if (structure.Restrictions == null)
        {
            structure.Restrictions = new LocationParamStructure();
        }
        structure.Restrictions.Type = locationTypeFilter.Select(t => t.FromLocationType()).ToList();
        return structure;
    }
}
