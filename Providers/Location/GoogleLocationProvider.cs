using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Exceptions;
using Newtonsoft.Json;

namespace Providers.Location
{
    public class GoogleLocationProvider : ILocationProvider
    {
        private readonly string _apikey;
        public GoogleLocationProvider(string apiKey)
        {
            _apikey = apiKey;
        }

        public async Task<LocationData> GetLocationDataAsync(string code)
        {
            var client = new WebClient { Encoding = Encoding.UTF8 };
            var str = await client.DownloadStringTaskAsync($"https://maps.googleapis.com/maps/api/place/details/json?placeid={code}&language=en_US&key={_apikey}");
            var data = JsonConvert.DeserializeObject<dynamic>(str);

            if (data.status == "INVALID_REQUEST")
            {
                throw new ResourceUnauthorizedException();
            }
            return GetLocationDataFromDataString(data.result, code);
        }

        public LocationData GetLocationDataFromDataString(string str, string code)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(str);
            return GetLocationDataFromDataString(data, code);
        }

        public LocationData GetLocationDataFromDataString(dynamic data, string code)
        {
            var location = new LocationData
            {
                Latitude = double.Parse(data.geometry.location.lat.ToString()),
                Longitude = double.Parse(data.geometry.location.lng.ToString())
            };
            foreach (var item in data.address_components)
            {
                List<string> types = item.types.ToObject<List<string>>();

                if (types.Contains(@"country"))
                {
                    location.Country = item.long_name;
                }
                if (types.Contains(@"locality"))
                {
                    location.City = item.long_name;
                }

                if (types.Contains(@"administrative_area_level_1"))
                {
                    location.State = item.short_name;
                }
                if (types.Contains(@"postal_code"))
                {
                    location.PostalCode = item.long_name;
                }
            }
            location.Code = code;
            location.FormattedAddress = data.formatted_address.ToString();
            return location;
        }

    }
}
