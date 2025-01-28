using System.Linq;
using TRIAS.NET.Models.Trias;

namespace TRIAS.NET.Models;
public record class Location
{
    public LocationType LocationType { get; set; }
    public Coordinates Coordinates { get; set; }
    public string Name { get; set; }
    public string LocationName { get; set; }
}

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
        }
        else if (item is StopPlaceStructure stopPlace)
        {
            location.LocationType = LocationType.StopPlace;
            location.Name = stopPlace.StopPlaceName.First().Text;
        }
        else if (item is LocalityStructure locality)
        {
            location.LocationType = LocationType.Locality;
            location.Name = locality.LocalityName.First().Text;
        }
        else if (item is PointOfInterestStructure pointOfInterest)
        {
            location.LocationType = LocationType.PointOfInterest;
            location.Name = pointOfInterest.PointOfInterestName.First().Text;
        }
        else if (item is AddressStructure address)
        {
            location.LocationType = LocationType.Address;
            location.Name = address.AddressName.First().Text;
        }
        return location;
    }
}