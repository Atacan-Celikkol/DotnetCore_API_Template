using System.Threading.Tasks;

namespace Providers.Location
{
    public interface ILocationProvider
    {
        Task<LocationData> GetLocationDataAsync(string code);
        LocationData GetLocationDataFromDataString(string str, string code);
        LocationData GetLocationDataFromDataString(dynamic data, string code);
    }

    public class LocationData
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PostalCode { get; set; }
        public string Code { get; set; }
        public string FormattedAddress { get; set; }
    }
}
