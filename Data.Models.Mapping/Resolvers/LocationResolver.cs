using GeoAPI.Geometries;
using NetTopologySuite;

namespace Data.Models.Mapping.Resolvers
{
    public static class LocationResolver
    {
        public static IPoint CreatePoint(double longitude, double latitude)
        {
            var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var location = factory.CreatePoint(new Coordinate(longitude, latitude));
            return location;
        }
    }
}